using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class UserController : ControllerBase
    {
        private readonly IReliableStateManager stateManager;

        public UserController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        // GET api/user/5
        [HttpGet("{id}")]
        public ActionResult<Model.UserData> Get(string id)
        {
            var userData = new Model.UserData();
            userData.Id = id;

            var userDataDictionary = stateManager.GetOrAddAsync<IReliableDictionary<string, Model.UserData>>(
                ReliableObjectNames.UserDataDictionary)
                .GetAwaiter()
                .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                var userinfo = userDataDictionary.TryGetValueAsync(
                    txn,
                    id)
                    .GetAwaiter()
                    .GetResult();

                if (userinfo.HasValue)
                {
                    userData.Pin = userinfo.Value.Pin;
                    userData.DeviceList = userinfo.Value.DeviceList;
                    userData.ForecastDataJson =
                        ReliableCollectionHelper.FetchForecastDataJsonObj(
                            stateManager,
                            userData.Pin);
                }
                else
                {
                    // TODO: Return 404 error.
                    userData.ForecastDataJson = "Unkown";
                    Telemetry.Client.TrackTrace($"User with Id {id} does not exist.");
                }
            }

            return userData;
        }

        // POST api/user/5
        [HttpPost("{id}")]
        public void Post(string id, [FromBody] Model.UserData value)
        {
            if (value == null ||
                string.IsNullOrWhiteSpace(value.Id))
            {
                return;
            }

            var userDataDictionary = stateManager.GetOrAddAsync<IReliableDictionary<string, Model.UserData>>(
                ReliableObjectNames.UserDataDictionary)
                .GetAwaiter()
                .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                userDataDictionary.SetAsync(
                    txn,
                    value.Id,
                    value)
                    .GetAwaiter()
                    .GetResult();

                txn.CommitAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            ReliableCollectionHelper.AddPinToForecastCollection(
                stateManager,
                value.Pin);
        }

        // POST api/user
        [HttpPost]
        public void Post([FromBody] Model.UserData data)
        {
            if (data == null ||
                string.IsNullOrWhiteSpace(data.Id))
            {
                return;
            }

            var userDataDictionary = stateManager.GetOrAddAsync<IReliableDictionary<string, Model.UserData>>(
                ReliableObjectNames.UserDataDictionary)
                .GetAwaiter()
                .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                userDataDictionary.SetAsync(
                    txn,
                    data.Id,
                    data)
                    .GetAwaiter()
                    .GetResult();

                txn.CommitAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            ReliableCollectionHelper.AddPinToForecastCollection(
                stateManager,
                data.Pin);
        }

        // PUT api/user/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] Model.UserData value)
        {
            if (value == null ||
                string.IsNullOrWhiteSpace(value.Id))
            {
                return;
            }

            var userDataDictionary = stateManager.GetOrAddAsync<IReliableDictionary<string, Model.UserData>>(
                ReliableObjectNames.UserDataDictionary)
                .GetAwaiter()
                .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                userDataDictionary.SetAsync(
                    txn,
                    value.Id,
                    value)
                    .GetAwaiter()
                    .GetResult();

                txn.CommitAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            ReliableCollectionHelper.AddPinToForecastCollection(
                stateManager,
                value.Pin);
        }

        // PUT api/user
        [HttpPut]
        public void Put([FromBody] Model.UserData data)
        {
            if (data == null ||
                string.IsNullOrWhiteSpace(data.Id))
            {
                return;
            }

            var userDataDictionary = stateManager.GetOrAddAsync<IReliableDictionary<string, Model.UserData>>(
                ReliableObjectNames.UserDataDictionary)
                .GetAwaiter()
                .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                userDataDictionary.SetAsync(
                    txn,
                    data.Id,
                    data)
                    .GetAwaiter()
                    .GetResult();

                txn.CommitAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            ReliableCollectionHelper.AddPinToForecastCollection(
                stateManager,
                data.Pin);
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            var userDataDictionary = stateManager.GetOrAddAsync<IReliableDictionary<string, Model.UserData>>(
                ReliableObjectNames.UserDataDictionary)
                .GetAwaiter()
                .GetResult();

            using (var txn = stateManager.CreateTransaction())
            {
                userDataDictionary.TryRemoveAsync(txn, id)
                    .GetAwaiter()
                    .GetResult();

                txn.CommitAsync()
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
