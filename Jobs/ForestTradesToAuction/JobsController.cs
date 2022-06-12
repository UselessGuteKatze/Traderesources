using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Yoda.Application.Queries;
using YodaApp.DbQueues;


namespace ForestTradesToAuction {
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

            //ForestTradesJobs.ForestAgreementsToAuctionJob.AddImmediately(new ForestTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(2023, 1, 1), new DateTime(2022, 1, 1)));
            ForestTradesJobs.ForestTradesToAuctionJob.AddImmediately(new ForestTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(3000, 1, 1), new DateTime(2021, 1, 1)));
            ForestTradesJobs.WaitingForestTradesFromAuctionJob.AddImmediately(new ForestTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(3000, 1, 1), new DateTime(2021, 1, 1)));
            ForestTradesJobs.HeldForestTradesFromAuctionJob.AddImmediately(new ForestTradesJobInput(), _queryExecuter, null, new JobSettings(new DateTime(3000, 1, 1), new DateTime(2021, 1, 1)));

            return new JsonResult(new
            {
                Text = "Done",
                Timestamp = DateTime.Now
            });
        }
    }

}
