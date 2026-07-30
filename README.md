# LevelDevil

LevelDevil 是以 Unity 2023.2.22f1 製作的 2D 平台動作專案。它保留既有「選 Map → 連續遊玩多個 Level 段落」的玩法，同時提供可擴充的關卡目錄、模板、驗證與快速測試工作流程。

![LevelDevil](https://github.com/user-attachments/assets/8963765c-4526-4d34-81ce-72b21f0d83ce)

## 目前能力

- 18 個既有 Map 與 81 個唯一 Level Prefab 維持原本的載入與遊玩流程。
- `LevelCatalog.asset` 提供 Map 與 Level 的集中資料：穩定 ID、顯示名稱、難度、Prefab 引用。
- `LevelTemplate.prefab` 為新關卡提供固定的 `LevelRoot`、`PlayerSpawn`、`Gate`、`CameraBounds`、`Geometry`、`Traps` 結構。
- Level Catalog Validator 可檢查 Map / Level 資料、Template 完整性、Gate、Spawn、Camera Bounds 與 Trap 的損壞引用。
- 可從 Project 視窗直接進入指定 Level 的 Play Mode 預覽。

## 環境與開啟方式

1. 安裝 Unity `2023.2.22f1`。
2. 以 Unity Hub 開啟本資料夾。
3. 開啟 `Assets/Scenes/SampleScene.unity`。
4. 在 Play Mode 從主選單進入選關，確認既有內容。

主要入口：

- 場景：`Assets/Scenes/SampleScene.unity`
- Catalog：`Assets/_Game/Data/LevelCatalog.asset`
- 新關卡模板：`Assets/_Game/Prefabs/LevelPrefab/LevelTemplate.prefab`
- 驗證工具：`Tools > LevelDevil > Level Catalog Validator`

## 新增 Level

1. 複製 `LevelTemplate.prefab`，以有意義的名稱儲存於 `Assets/_Game/Prefabs/LevelPrefab/`。
2. 保留 Template 的 `LevelRoot`、`PlayerSpawn`、`Gate`、`CameraBounds`；地形放在 `Geometry`，機關放在 `Traps`。
3. 以既有 `MapBuild`、`Trigger` 與機關 Prefab 組裝內容，並完成每個機關要求的 Inspector 引用。
4. 在 Project 視窗選取新 Level Prefab，執行 `Tools > LevelDevil > Play Selected Level`（Ctrl+Shift+P）立即測試。
5. 將通過測試的 Level 加入目標 `Map` Prefab 的 `levelList`，以維持既有 Map 順序與玩法。
6. 執行 `Tools > LevelDevil > Synchronize Catalog Level Entries`，自動同步 ID、顯示名稱與 Prefab 資料。
7. 執行 Catalog Validator，修正 Error 後再提交。

> Catalog 同步不會改寫 Map 的順序或 PlayerPrefs 進度；正式關卡順序仍由既有 `Map.levelList` 決定，這是保留向後相容性的設計。

## 驗證

每個關卡製作 Phase 完成後，依序執行：

1. Unity 編譯確認。
2. Play Mode 測試新 Level 與既有流程。
3. 執行 Level Catalog Validator。
4. 提交修改摘要，再進下一階段。

目前的 Play Mode 回歸測試覆蓋 18 個 Map、81 個唯一舊 Level、Catalog Level Metadata，以及快速預覽載入路徑。

## 專案結構

```text
Assets/
├─ Scenes/SampleScene.unity              # 常駐遊戲場景
├─ _Game/Data/LevelCatalog.asset          # Map / Level 目錄
├─ _Game/Prefabs/AllLevel/                # Map1..Map18 容器
├─ _Game/Prefabs/LevelPrefab/             # Level 與 LevelTemplate
├─ _Game/Prefabs/MapBuild/, Trigger/      # 可重用地形與機關
├─ _Game/Scripts/Level/                   # Level、Map、LevelManager
└─ _Game/Editor/                          # Validator、Catalog 同步、快速預覽
```
