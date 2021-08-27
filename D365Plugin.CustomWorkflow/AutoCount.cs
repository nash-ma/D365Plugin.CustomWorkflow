using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Reflection;

namespace D365Plugin.CustomWorkflow
{
    public class AutoCount : CodeActivity
    {
        // 入力
        [RequiredArgument]
        [Input("String input")]
        public InArgument<string> StringInput { get; set; }

        // 出力
        [Output("Integer output")]
        public OutArgument<decimal> TargetCountOutput { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // トレースサービスの取得
            ITracingService tracingService = context.GetExtension<ITracingService>();
            // コンテキストの取得
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            // 組織サービスの取得             
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            try
            {
                tracingService.Trace("処理開始...");

                // 入力内容のチェック

                // 入力内容の取得

                // 検索処理を行う

                // 出力内容の設定

                tracingService.Trace("処理終了...");
            }
            catch (Exception e)
            {
                // エラーを表示
                throw new InvalidPluginExecutionException(
                    $@"プラグイン名：{Assembly.GetExecutingAssembly().GetName()}
                   「
                    {e.Message}
                    」"
                     );
            }



        }
    }
}
