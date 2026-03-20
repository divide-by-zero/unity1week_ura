# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## プロジェクト概要

unity1week参加用のUnityプロジェクト（ゲームジャム）。現在は初期テンプレート状態。

## 環境

- **Unity**: 6000.3.10f1 (Unity 6)
- **レンダーパイプライン**: URP (Universal Render Pipeline)
  - PC用 / Mobile用の2つのRenderer設定あり (`Assets/Settings/`)
- **主要パッケージ**: Input System 1.18, Timeline, AI Navigation, Visual Scripting

## プロジェクト構成

- `Assets/Scenes/` - シーンファイル（現在 SampleScene のみ）
- `Assets/Settings/` - URP Renderer / Volume Profile 設定
- `Assets/InputSystem_Actions.inputactions` - Input System アクションマップ定義

## ビルド・開発

- Unity Editorから開いて操作する（CLIビルドは未設定）
- C#スクリプトは `Assets/` 以下に配置（Assembly-CSharp）
- テストフレームワーク (`com.unity.test-framework`) は導入済み

## uloop simulate-mouse の注意点

- `simulate-mouse` を実行する前に必ず `uloop focus-window` でUnity Editorをフォーカスすること
- フォーカスしないとクリックがUI要素にヒットしない
