using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Yoda.Application.Queries;
using YodaApp.DbQueues;


namespace LandTradesToAuction {
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

            LandTradesJobs.LandEgknIdToAuctionJob.AddImmediately(new LandTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2023, 1, 1), new DateTime(2022, 1, 1)));
            //LandTradesJobs.LandAgreementsToAuctionJob.AddImmediately(new LandTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2023, 1, 1), new DateTime(2022, 1, 1)));
            //LandTradesJobs.LandTradesToAuctionJob.AddImmediately(new LandTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));
            //LandTradesJobs.WaitingLandTradesFromAuctionJob.AddImmediately(new LandTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));
            //LandTradesJobs.HeldLandTradesFromAuctionJob.AddImmediately(new LandTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));

            return new JsonResult(new
            {
                Text = "Done",
                Timestamp = DateTime.Now
            });
        }
    }

}
