using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Yoda.Application.Queries;
using YodaApp.DbQueues;


namespace HuntingTradesToAuction {
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

            HuntingTradesJobs.HuntingAgreementsToAuctionJob.AddImmediately(new HuntingTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2023, 1, 1), new DateTime(2022, 1, 1)));
            //HuntingTradesJobs.HuntingTradesToAuctionJob.AddImmediately(new HuntingTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));
            //HuntingTradesJobs.WaitingHuntingTradesFromAuctionJob.AddImmediately(new HuntingTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));
            //HuntingTradesJobs.HeldHuntingTradesFromAuctionJob.AddImmediately(new HuntingTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2022, 1, 1), new DateTime(2021, 1, 1)));

            return new JsonResult(new
            {
                Text = "Done",
                Timestamp = DateTime.Now
            });
        }
    }

}
