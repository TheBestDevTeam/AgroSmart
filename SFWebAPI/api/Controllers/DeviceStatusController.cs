using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class DeviceStatusController : ControllerBase
    {
        private readonly IReliableStateManager stateManager;

        public DeviceStatusController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public ActionResult<Model.DeviceStatus> Get(string id)
        {
            var status = new Model.DeviceStatus();
            if(string.IsNullOrWhiteSpace(id))
            {
                return status;
            }

            var devStatusCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, Model.DeviceStatus>>(
                    ReliableObjectNames.DeviceStatusDictionary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                var devData = devStatusCollection.TryGetValueAsync(txn, id).GetAwaiter().GetResult();
                if (devData.HasValue)
                {
                    status = devData.Value;
                }
                else
                {
                    status.Id = id;
                    status.Status = "Unknown";
                }
            }

            return status;
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]Model.DeviceStatus data)
        {
            if (data == null ||
                string.IsNullOrWhiteSpace(data.Id))
            {
                return;
            }

            var devStatusCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, Model.DeviceStatus>>(
                    ReliableObjectNames.DeviceStatusDictionary)
                    .GetAwaiter()
                    .GetResult();

            var status = new Model.DeviceStatus();
            using (var txn = stateManager.CreateTransaction())
            {
                devStatusCollection.SetAsync(txn, data.Id, data).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        public void Put([FromBody]Model.DeviceStatus data)
        {
            if (data == null ||
                string.IsNullOrWhiteSpace(data.Id))
            {
                return;
            }

            var devStatusCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, Model.DeviceStatus>>(
                    ReliableObjectNames.DeviceStatusDictionary)
                    .GetAwaiter()
                    .GetResult();

            var status = new Model.DeviceStatus();
            using (var txn = stateManager.CreateTransaction())
            {
                devStatusCollection.SetAsync(txn, data.Id, data).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            var devStatusCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, Model.DeviceStatus>>(
                    ReliableObjectNames.DeviceStatusDictionary)
                    .GetAwaiter()
                    .GetResult();

            var status = new Model.DeviceStatus();
            using (var txn = stateManager.CreateTransaction())
            {
                devStatusCollection.TryRemoveAsync(txn, id).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }
    }
}
