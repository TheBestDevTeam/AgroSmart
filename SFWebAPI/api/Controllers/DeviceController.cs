using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IReliableStateManager stateManager;

        public DeviceController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        // GET api/device/5
        [HttpGet("{id}")]
        public ActionResult<Model.DeviceData> Get(string id)
        {
            var dev = new DeviceData();
            if (string.IsNullOrWhiteSpace(id))
            {
                return dev;
            }

            var devDataCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceData>>(
                    ReliableObjectNames.DeviceDataDictonary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                var devData = devDataCollection.TryGetValueAsync(txn, id).GetAwaiter().GetResult();
                if(devData.HasValue)
                {
                    dev = devData.Value;
                }
            }

            return dev;
        }

        // POST api/device/5
        [HttpPost("{id}")]
        public void Post(string id, [FromBody] Model.DeviceData value)
        {
            if (value == null ||
                string.IsNullOrWhiteSpace(value.Id))
            {
                return;
            }

            var devDataCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceData>>(
                    ReliableObjectNames.DeviceDataDictonary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                devDataCollection.SetAsync(txn,value.Id,value).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }

        // POST api/device
        [HttpPost]
        public void Post([FromBody] Model.DeviceData data)
        {
            if (data == null ||
                string.IsNullOrWhiteSpace(data.Id))
            {
                return;
            }

            var devDataCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceData>>(
                    ReliableObjectNames.DeviceDataDictonary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                devDataCollection.SetAsync(txn, data.Id, data).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }

        // PUT api/device/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] Model.DeviceData value)
        {
            if (value == null ||
                string.IsNullOrWhiteSpace(value.Id))
            {
                return;
            }

            var devDataCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceData>>(
                    ReliableObjectNames.DeviceDataDictonary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                devDataCollection.SetAsync(txn, value.Id, value).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }

        // PUT api/device
        [HttpPut]
        public void Put([FromBody] Model.DeviceData data)
        {
            if (data == null ||
                string.IsNullOrWhiteSpace(data.Id))
            {
                return;
            }

            var devDataCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceData>>(
                    ReliableObjectNames.DeviceDataDictonary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                devDataCollection.SetAsync(txn, data.Id, data).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }

        // DELETE api/device/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            var devDataCollection =
                stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceData>>(
                    ReliableObjectNames.DeviceDataDictonary)
                    .GetAwaiter()
                    .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                devDataCollection.TryRemoveAsync(txn, id).GetAwaiter().GetResult();
                txn.CommitAsync().GetAwaiter().GetResult();
            }
        }
    }
}
