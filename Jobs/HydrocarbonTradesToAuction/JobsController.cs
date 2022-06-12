using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Yoda.Application.Queries;
using YodaApp.DbQueues;


namespace HydrocarbonTradesToAuction {
    [ApiController]
    [Route("[controller]")]
    public class JobsController : Controller {

        private readonly IQueryExecuterSu _queryExecuter;

        public JobsController(ILogger<JobsController> logger, IQueryExecuterProvider queryExecuterProvider)
        {
            _queryExecuter = queryExecuterProvider.CreateQueryExecuterSuperUser();
        }

        [HttpGet]
        [Route("Ping")]
        public JsonResult Ping()
        {
            return new JsonResult(new
            {
                Text = "Hello world",
                Timestamp = DateTime.Now
            });
        }

        [HttpGet]
        [Route("create-job")]
        public JsonResult CreateTransferReportsJob()
        {

            //HydrocarbonTradesJobs.HydrocarbonAgreementsToAuctionJob.AddImmediately(new HydrocarbonTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2023, 1, 1), new DateTime(2022, 1, 1)));
            HydrocarbonTradesJobs.HydrocarbonTradesToAuctionJob.AddImmediately(new HydrocarbonTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));
            HydrocarbonTradesJobs.WaitingHydrocarbonTradesFromAuctionJob.AddImmediately(new HydrocarbonTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));
            HydrocarbonTradesJobs.HeldHydrocarbonTradesFromAuctionJob.AddImmediately(new HydrocarbonTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));

            return new JsonResult(new
            {
                Text = "Done",
                Timestamp = DateTime.Now
            });
        }
    }

}
