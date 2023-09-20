# BubbleConverter
Mermaidで書いた状態遷移図から，unity用のステートマシンを生成するプログラム
## 導入方法
### 事前準備
VSCodeでMermaidがプレビューできるように，以下のページを参考に拡張機能を追加してください．
[VSCodeでMermaidを扱う為の便利な拡張機能あれこれ](https://qiita.com/sato_kana/items/2a13f19017576488f017)
### Bubble Converter の導入
1. Unity Editor で`Window>Package Manager` を開き，左上の+ボタンから"Add package from git URL" を選択．![スクリーンショット (52)](https://github.com/iodoform/BubbleConverter/assets/63995635/290fd40f-95ef-475c-af4b-a6a197262ed0)

1. 入力欄に`https://github.com/iodoform/BubbleConverter.git?path=Packages/bubble_converter`と入力して`Add`をクリック．これで導入完了です．
## 使用方法
### ステートマシンの生成
.mdで書いた状態遷移図からステートマシンを生成
1.  上のメニューバーに`Bubble Converter`という項目が追加されるので，`Bubble Converter>Bubble Converter Window` をクリックしてタブを追加．
1. Assetフォルダ以下の適当な場所に，状態遷移図を書いた.mdファイルを作成．.mdファイルはUnity上では追加できないため，エクスプローラーかコードエディタで追加． 
    ![スクリーンショット (55)](https://github.com/iodoform/BubbleConverter/assets/63995635/403cdc9e-3530-41c7-bd87-44af32e4ebd7)
1.  Bubble Converterタブで，`Select Input File`に.mdファイル，`Select Output Folder`にAsset以下の適当なフォルダを選択．`State Machine Name`に半角英数字で名前を入力して`Convert File`を押すと，`<output folder path>/<state machine name>`以下にC#スクリプトが生成され，シーンにステートマシンが追加されます．  
    ![スクリーンショット (57)](https://github.com/iodoform/BubbleConverter/assets/63995635/949f444c-2daa-4e4b-949f-ddbba05da41b)
1.  `<output folder path>/<state machine name>`を開くと各状態名のファイルと，`StateMachine.cs`，`StateMachineTriggerMrthods.cs`，`DataCenter.cs`が追加されていると思うので，各状態での振る舞いをその状態の名前の付いたファイルに，遷移条件を`StateMachineTriggerMrthods.cs`に記述してください．`StateMachine.cs`は基本的にいじらないようにしてください．`DataCenter.cs`には，状態間で共有する変数を記述してください．  
    ![スクリーンショット (61)](https://github.com/iodoform/BubbleConverter/assets/63995635/5de861f2-0edd-4fd4-8947-92695c000ed2)

### ステートマシンの更新
.mdファイルの状態遷移図の書き換えを既存のステートマシンに反映します．
1. `Select Reconvert Folder`から仕様変更するステートマシンのスクリプトファイルがあるフォルダ（この画像だと`Scenes/Test`）を選択して `Reconvert File`を押す．
1. もとになる.mdファイル自体を差し替えたい場合は，`Select Input File`のパスを書き換えてから同様の手順で`Reconvert File`を押す．![スクリーンショット (59)](https://github.com/iodoform/BubbleConverter/assets/63995635/7c47f5d2-d97b-46bf-b82f-f7bbe802f2db)
### 参考
ステートマシンの本体部分は以下のページで公開されているコードを参考に実装しています．
- [【Unity】より汎用的な有限ステートマシンを実装する](https://light11.hatenadiary.com/entry/2019/02/14/223312)
