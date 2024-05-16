using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Promotion;
using Baetoti.Shared.Extentions;
using System.Linq;

namespace Baetoti.API.Controllers
{
	public class PromotionController : ApiBaseController
	{
		public readonly IPromotionRepository _promotionRepository;
		public readonly IUserPromotionRepository _userpromotionRepository;
		public readonly ICartRepository _cartRepository;
		public readonly ITransactionRepository _transactionRepository;
		public readonly IMapper _mapper;

		public PromotionController(
			IPromotionRepository promotionRepository,
			IUserPromotionRepository userpromotionRepository,
			ICartRepository cartRepository,
			ITransactionRepository transactionRepository,
			IMapper mapper
			)
		{
			_promotionRepository = promotionRepository;
			_userpromotionRepository = userpromotionRepository;
			_cartRepository = cartRepository;
			_transactionRepository = transactionRepository;
			_mapper = mapper;
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add([FromBody] PromotionRequest promotionRequest)
		{
			try
			{
				var promotion = _mapper.Map<Promotion>(promotionRequest);
				promotion.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
				promotion.CreatedBy = Convert.ToInt32(UserId);
				promotion.PromotionStatus = (int)PromotionStatus.Active;
				var result = await _promotionRepository.AddAsync(promotion);
				if (result == null)
				{
					return Ok(new SharedResponse(false, 400, "Unable To Create Promotion"));
				}
				return Ok(new SharedResponse(true, 200, "Promotion Created Succesfully"));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}

		[HttpGet("View")]
		public async Task<IActionResult> View(long PromotionID)
		{
			try
			{
				var promotionsData = await _promotionRepository.View(PromotionID);
				return Ok(new SharedResponse(true, 200, "", promotionsData));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message, null));
			}
		}

		[HttpPost("GetAll")]
		public async Task<IActionResult> GetAll([FromBody] GetPromotionRequest getPromotionRequest)
		{
			try
			{
				var promotionsData = await _promotionRepository.GetAll(getPromotionRequest);
				return Ok(new SharedResponse(true, 200, "", promotionsData));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message, null));
			}
		}

		[HttpPost("UpdateStatus")]
		public async Task<IActionResult> UpdateStatus([FromBody] PromotionStatusChangeRequest request)
		{
			try
			{
				var promotion = await _promotionRepository.GetByIdAsync(request.ID);
				promotion.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
				promotion.UpdatedBy = Convert.ToInt32(UserId);
				promotion.PromotionStatus = request.StatusValue;
				await _promotionRepository.UpdateAsync(promotion);
				return Ok(new SharedResponse(true, 200, "Promotion Status Updated Successfully"));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}


		[HttpPost("GetDiscount")]
		public async Task<IActionResult> GetDiscount(string request)
		{
			try
			{
				var promotion = await _promotionRepository.ListAllAsync();
				var getpromotion = promotion.Where(ww => ww.PromoCodeName.Contains(request)).FirstOrDefault();
				if(getpromotion != null)
                {
					if(getpromotion.PromoCodeValidity < DateTime.Now)
                    {
						var userpromotion = await _userpromotionRepository.ListAllAsync();
						var checkuserpromotioncode = userpromotion.Where(ww => ww.PromotionID == getpromotion.ID).FirstOrDefault();
						if(checkuserpromotioncode == null)
                        {
							var cart = await _cartRepository.GetCartDetail(Convert.ToInt64(UserId));
							decimal total = 0;
							if(getpromotion.DiscountType == 1)
                            {
								//%
								 total = cart.TotalCharges * (getpromotion.DiscountValue / 100);
								 total = cart.TotalCharges - total;
								
                            }
                            else
                            {
								//value
								 total = cart.TotalCharges - getpromotion.DiscountValue;
								
                            }

							return Ok(new SharedResponse(true, 200, "Promotion code use succesfully", total));
						}
						else
                        {
							return Ok(new SharedResponse(false, 200, "Promotion code is already used"));
						}
                    }
                    else
                    {
						return Ok(new SharedResponse(false, 200, "Promotion code is expired"));
					}
                }
                else
                {
					return Ok(new SharedResponse(false, 200, "Promotion code not found"));
				}
				 
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}


	}
}
