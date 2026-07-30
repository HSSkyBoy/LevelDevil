using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// Android-only, offline release signer pinning. It compares the installed APK
/// signing certificate to the SHA-256 configured in AndroidSignatureValidationSettings.
/// It intentionally performs no environment, device, root, emulator, or network checks.
/// </summary>
public static class AndroidSignatureValidator
{
    private const string SettingsResourceName = "AndroidSignatureValidationSettings";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ValidateReleaseSigner()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidSignatureValidationSettings settings = Resources.Load<AndroidSignatureValidationSettings>(SettingsResourceName);
        if (settings == null || !settings.HasExpectedCertificate)
        {
            return;
        }

        if (Debug.isDebugBuild && !settings.EnforceInDevelopmentBuilds)
        {
            return;
        }

        string actualCertificate;
        if (!TryGetSigningCertificateSha256(out actualCertificate) ||
            !string.Equals(actualCertificate, settings.GetNormalizedExpectedCertificate(), StringComparison.Ordinal))
        {
            Reject("Android APK signing certificate does not match the configured release pin.");
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static bool TryGetSigningCertificateSha256(out string certificateSha256)
    {
        certificateSha256 = null;

        try
        {
            using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
            using (AndroidJavaClass flags = new AndroidJavaClass("android.content.pm.PackageManager"))
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager"))
            {
                string packageName = activity.Call<string>("getPackageName");
                int sdkInt = version.GetStatic<int>("SDK_INT");
                int signingFlags = sdkInt >= 28
                    ? flags.GetStatic<int>("GET_SIGNING_CERTIFICATES")
                    : flags.GetStatic<int>("GET_SIGNATURES");

                using (AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, signingFlags))
                {
                    AndroidJavaObject[] signatures = GetSignatures(packageInfo, sdkInt);
                    if (signatures == null || signatures.Length == 0 || signatures[0] == null)
                    {
                        return false;
                    }

                    using (AndroidJavaObject firstSignature = signatures[0])
                    {
                        byte[] certificateBytes = firstSignature.Call<byte[]>("toByteArray");
                        if (certificateBytes == null || certificateBytes.Length == 0)
                        {
                            return false;
                        }

                        using (SHA256 sha256 = SHA256.Create())
                        {
                            certificateSha256 = ToHex(sha256.ComputeHash(certificateBytes));
                        }

                        return true;
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Android signing-certificate verification failed: " + exception.Message);
            return false;
        }
    }

    private static AndroidJavaObject[] GetSignatures(AndroidJavaObject packageInfo, int sdkInt)
    {
        if (sdkInt >= 28)
        {
            using (AndroidJavaObject signingInfo = packageInfo.Get<AndroidJavaObject>("signingInfo"))
            {
                return signingInfo != null
                    ? signingInfo.Call<AndroidJavaObject[]>("getApkContentsSigners")
                    : null;
            }
        }

        return packageInfo.Get<AndroidJavaObject[]>("signatures");
    }

    private static string ToHex(byte[] bytes)
    {
        StringBuilder builder = new StringBuilder(bytes.Length * 2);
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }

        return builder.ToString();
    }

    private static void Reject(string reason)
    {
        Debug.LogError(reason);
        Application.Quit();
    }
#endif
}
