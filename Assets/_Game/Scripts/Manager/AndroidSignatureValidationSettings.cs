using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AndroidSignatureValidationSettings", menuName = "LevelDevil/Android Signature Validation Settings")]
public sealed class AndroidSignatureValidationSettings : ScriptableObject
{
    [Tooltip("Optional SHA-256 of the release signing certificate in lowercase hexadecimal. Leave empty to build and run without signature validation.")]
    [SerializeField] private string expectedCertificateSha256;
    [SerializeField] private bool enforceInDevelopmentBuilds;

    public bool EnforceInDevelopmentBuilds => enforceInDevelopmentBuilds;

    public bool HasExpectedCertificate => GetNormalizedExpectedCertificate().Length == 64;

    public string GetNormalizedExpectedCertificate()
    {
        return string.IsNullOrEmpty(expectedCertificateSha256)
            ? string.Empty
            : expectedCertificateSha256.Replace(":", string.Empty).Replace(" ", string.Empty).Trim().ToLowerInvariant();
    }
}
