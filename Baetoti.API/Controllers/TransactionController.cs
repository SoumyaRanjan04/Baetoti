using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Request.Transaction;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class TransactionController : ApiBaseController
    {

        public readonly ITransactionRepository _transactionRepository;
        public readonly IMapper _mapper;

        public TransactionController(
            ITransactionRepository transactionRepository,
            IMapper mapper
            )
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var usersData = await _transactionRepository.GetAll();
                return Ok(new SharedResponse(true, 200, "", usersData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetFilteredData")]
        public async Task<IActionResult> GetFilteredData([FromBody] TransactionFilterRequest request)
        {
            try
            {
                var transactionsData = await _transactionRepository.GetFitered(request);
                return Ok(new SharedResponse(true, 200, "", transactionsData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(long Id, int UserType)
        {
            try
            {
                var transactionData = await _transactionRepository.GetByID(Id, UserType);
                return Ok(new SharedResponse(true, 200, "", transactionData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
