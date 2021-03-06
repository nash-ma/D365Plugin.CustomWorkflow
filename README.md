# D365Plugin.CustomWorkflow
Dynamics365 ユーザー定義ワークフロー活動の作成(Create a custom workflow activities with Dynamics 365)


## ■要望
- 学生が新規作成時、所属クラスの学生人数を再計算してクラスの項目「学生人数」を更新する。　　　　　　　　　　
- 「クラス名」が更新時、変更前「クラス」と変更後「クラス」の学生人数を計算してそれぞれの項目「学生人数」を更新する。

### テーブル：学生台帳（pas_tbl_student）
| 表示名 | 名前 | データタイプ | 必須 | 備考1 | 備考2 |
|:---:|:---:|:---:|:---:|:---:|:---:|
|学生番号 |pas_student_no |一行テキスト |● |オートナンバー | | 
|クラス |pas_class |検索 |● |参照先：クラス管理 | | 

### テーブル：クラス管理（pas_tbl_class）
| 表示名 | 名前 | データタイプ | 必須 | 備考1 | 備考2 |
|:---:|:---:|:---:|:---:|:---:|:---:|
|クラス番号 |pas_class_no |一行テキスト |● | | | 
|学生人数 |pas_student_number |整数 | |範囲：0～25 | | 

## ■基本設計
- カスタムワークフローを利用し、常に指定された検索項目が関連している対象エンティティの件数を計算する。
- D365のWFという標準機能を利用し、学生が作成後または学生のクラスが変更後にカスタムワーク（D365Plugin.CustomWorkflow）を実行する。出力内容をクラスの学生人数に設定する。

## ■詳細設計
#### 〇カスタムワークフロー：
| プロパティ名 | データ種類 | 必須 | 値 | 既定値 |
|:---:|:---:|:---:|:---:|:---:
|入力内容：検索項目名 |テキスト |●| |pas_class|  
|出力内容：件数 |整数 |-| |-| 

#### 〇D365ワークフローの設定：
**①　変更前WF：**
- 開始時期：レコード フィールドの変更前（項目：クラス）
- 処理：
  - ステップ１：変更前の状態でカスタムWFを実行してクラスの「学生人数」を計算する。
  - ステップ２：関連クラスの項目「学生人数」がカスタムWFの再計算結果と一致しないかをチェック
  - ステップ３：２で一致しなければ、関連クラスの「学生人数」を更新する。
  - ステップ４：「学生人数」の値が1以上かをチェック
  - ステップ５：４で1以上であれば、「学生人数」をマイナス１にして関連クラスを更新する。


**②　作成後または変更後WF：**
- 開始時期：レコードの作成後、またはレコード フィールドの変更後（項目：クラス）
- 処理：
  - ステップ１：設定後の状態でカスタムWFを実行してクラスの「学生人数」を計算する。
  - ステップ２：クラスの項目「学生人数」を更新する。

## ■結果
- 〇プラグインの登録：
![プラグインの登録](D365Plugin.CustomWorkflow/image/プラグイン登録001.png "プラグインの登録")

- 〇ワークフローの作成：
![WF001](D365Plugin.CustomWorkflow/image/WF001.png "WF001")
![WF002](D365Plugin.CustomWorkflow/image/WF002.png "WF002")
![WF003](D365Plugin.CustomWorkflow/image/WF003.png "WF003")

- 〇学生作成時の前後：
![作成前001](D365Plugin.CustomWorkflow/image/作成前001.png "作成前001")
![作成前002](D365Plugin.CustomWorkflow/image/作成前002.png "作成前002")
![作成中001](D365Plugin.CustomWorkflow/image/作成中001.png "作成中001")
![作成後001](D365Plugin.CustomWorkflow/image/作成後001.png "作成後001")
![作成後002](D365Plugin.CustomWorkflow/image/作成後002.png "作成後002")

- 〇学生のクラス変更時の前後：
![変更前_A](D365Plugin.CustomWorkflow/image/変更前_A.png "変更前_A")
![変更前_B](D365Plugin.CustomWorkflow/image/変更前_B.png "変更前_B")
![変更A⇒B](D365Plugin.CustomWorkflow/image/変更A⇒B.png "変更A⇒B")
![変更後_A](D365Plugin.CustomWorkflow/image/変更後_A.png "変更後_A")
![変更後_B](D365Plugin.CustomWorkflow/image/変更後_B.png "変更後_B")


#### ■参考：
・[チュートリアル: ワークフロー拡張の作成](https://docs.microsoft.com/ja-jp/powerapps/developer/data-platform/workflow/tutorial-create-workflow-extension) <br>
・[CodeActivity クラス](https://docs.microsoft.com/ja-jp/dotnet/api/system.activities.codeactivity?view=netframework-4.8)
