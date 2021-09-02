using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Reflection;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.ObjectModel;

namespace D365Plugin.CustomWorkflow
{
    public class AutoCount : CodeActivity
    {
        // 入力
        [RequiredArgument]
        [Default("pas_class")]
        [Input("入力内容：検索項目名")]
        public InArgument<string> StringInput { get; set; }

        // 出力
        [Output("出力内容：件数")]
        public OutArgument<decimal> TargetCountOutput { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // トレースサービスの取得
            ITracingService tracingService = context.GetExtension<ITracingService>();
            // コンテキストの取得
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            // 組織サービスの取得             
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            tracingService.Trace("処理開始...");

            try
            {
                // 入力内容の取得
                string stringInput = StringInput.Get<string>(context);
                // トレースログを出力
                tracingService.Trace($"入力内容項目名：{stringInput}");

                // ターゲットエンティティを取得
                Entity entity = (Entity)workflowContext.InputParameters["Target"];
                // エンティティロジック名の取得
                String entityName = entity.LogicalName;
                // DBエンティティデータの取得
                Entity DbEntity = service.Retrieve(entityName, entity.Id, new ColumnSet(true));
                // トレースログを出力
                tracingService.Trace("DBエンティティデータの取得が完了");

                // 入力内容のチェック(ターゲットエンティティに含まれているか)
                if (!DbEntity.Attributes.Contains(stringInput))
                    throw new Exception("入力内容がエンティティに含まれていないので、再確認してください！");

                // 入力内容のチェック(入力内容が設定されているか)
                if (DbEntity.GetAttributeValue<EntityReference>(stringInput) == null || DbEntity.GetAttributeValue<EntityReference>(stringInput).Id == new Guid())
                    throw new Exception("入力内容が設定されていないので、再確認してください！");

                // 検索項目GUID
                Guid LookupGuid = DbEntity.GetAttributeValue<EntityReference>(stringInput).Id;

                // 検索処理を行う
                tracingService.Trace("検索処理を行う");
                QueryByAttribute query = new QueryByAttribute();
                // 主エンティティ名
                query.EntityName = entityName;
                // 結果列：既存以外は無し
                query.ColumnSet = new ColumnSet(false);
                // フィルター列
                query.Attributes.AddRange($"{stringInput}");
                // 列の値
                query.Values.AddRange(new object[] { LookupGuid });

                // 検索を行う
                RetrieveMultipleRequest request = new RetrieveMultipleRequest();
                request.Query = query;
                Collection<Entity> entityCollection = ((RetrieveMultipleResponse)service.Execute(request)).EntityCollection.Entities;

                // 出力内容に設定
                TargetCountOutput.Set(context, entityCollection.Count);
                tracingService.Trace($"出力内容：{entityCollection.Count}");
                tracingService.Trace("処理正常終了...");

            }
            catch (Exception e)
            {
                // エラーを表示
                throw new InvalidOperationException(
                    $@"プラグイン名：{Assembly.GetExecutingAssembly().GetName()}
                   「
                    {e.Message}
                    」"
                     );
            }



        }
    }
}
