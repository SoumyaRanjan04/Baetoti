using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Cart;
using Baetoti.Infrastructure.Data.Repositories;
using Baetoti.Shared.Response.Dashboard;
using Baetoti.Shared.Response.Category;
using System.Collections.Generic;
using Baetoti.Shared.Extentions;

namespace Baetoti.API.Controllers
{
    public class CartController : ApiBaseController
    {

        public readonly ICartRepository _cartRepository;
        public readonly IItemRepository _itemRepository;
        public readonly IMapper _mapper;

        public CartController(
            ICartRepository cartRepository,
            IItemRepository itemRepository,
            IMapper mapper
            )
        {
            _cartRepository = cartRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var cart = await _cartRepository.GetCartDetail(Convert.ToInt64(UserId));
                return Ok(new SharedResponse(true, 200, "", cart));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(long ItemID)
        {
            try
            {
                if (await _cartRepository.CheckForStore(ItemID, Convert.ToInt64(UserId)))
                {
                    var item = await _itemRepository.GetByIdAsync(ItemID);
                    var cartAlreadyExist = await _cartRepository.GetByItemID(ItemID, Convert.ToInt64(UserId));
                    if (cartAlreadyExist == null)
                    {
                        if (item.IsCheckQuantity && item.AvailableQuantity == 0)
                        {
                            return Ok(new SharedResponse(false, 400, "This item is not availabe in the stock."));
                        }
                        Cart cart = new Cart
                        {
                            Quantity = 1,
                            ItemID = ItemID,
                            UserID = Convert.ToInt64(UserId),
                            RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                        };
                        var result = await _cartRepository.AddAsync(cart);
                        if (result == null)
                        {
                            return Ok(new SharedResponse(false, 400, "Unable To Add"));
                        }
                        var cartDetail = await _cartRepository.GetCartDetail(Convert.ToInt64(UserId));
                        return Ok(new SharedResponse(true, 200, "Item Added Successfully", cartDetail));
                    }
                    else
                    {
                        if (item.IsCheckQuantity && item.AvailableQuantity <= cartAlreadyExist.Quantity)
                        {
                            return Ok(new SharedResponse(false, 400, $"Only {item.AvailableQuantity} item(s) are availbe in the stock."));
                        }
                        cartAlreadyExist.Quantity = cartAlreadyExist.Quantity + 1;
                        await _cartRepository.UpdateAsync(cartAlreadyExist);
                        var cartDetail = await _cartRepository.GetCartDetail(Convert.ToInt64(UserId));
                        return Ok(new SharedResponse(true, 200, "Quantity Updated Successfully", cartDetail));
                    }
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "An item already exists in cart from another store. Please remove that and try again."));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Delivery config is not defined for"))
                {
                    var cart = await _cartRepository.GetByUserID(Convert.ToInt64(UserId));
                    if (cart != null)
                    {
                        await _cartRepository.DeleteRangeAsync(cart);
                    }
                }
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] CartRequest cartRequest)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(cartRequest.ItemID);
                if (item.IsCheckQuantity && item.AvailableQuantity < cartRequest.Quantity)
                {
                    return Ok(new SharedResponse(false, 400, $"Only {item.AvailableQuantity} item(s) are availbe in the stock."));
                }
                var cart = await _cartRepository.GetByItemID(cartRequest.ItemID, Convert.ToInt64(UserId));
                cart.Quantity = cartRequest.Quantity;
                await _cartRepository.UpdateAsync(cart);
                var cartDetail = await _cartRepository.GetCartDetail(Convert.ToInt64(UserId));
                return Ok(new SharedResponse(true, 200, "Quantity Updated Successfully", cartDetail));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpDelete("Remove/{ItemID}")]
        public async Task<IActionResult> Remove(long ItemID, bool IsRemoveItem)
        {
            try
            {
                var cart = await _cartRepository.GetByItemID(ItemID, Convert.ToInt64(UserId));
                if (cart != null)
                {
                    if (IsRemoveItem)
                    {
                        await _cartRepository.DeleteAsync(cart);
                        return Ok(new SharedResponse(true, 200, "Item Removed Succesfully"));
                    }
                    else
                    {
                        if (cart.Quantity > 1)
                        {
                            cart.Quantity = cart.Quantity - 1;
                            await _cartRepository.UpdateAsync(cart);
                            var cartDetail = await _cartRepository.GetCartDetail(Convert.ToInt64(UserId));
                            return Ok(new SharedResponse(true, 200, "Quantity Updated Succesfully", cartDetail));
                        }
                        else
                        {
                            await _cartRepository.DeleteAsync(cart);
                            return Ok(new SharedResponse(true, 200, "Item Removed Succesfully"));
                        }
                    }
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Item"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpDelete("RemoveAll")]
        public async Task<IActionResult> RemoveAll()
        {
            try
            {
                var cart = await _cartRepository.GetByUserID(Convert.ToInt64(UserId));
                if (cart != null)
                {
                    await _cartRepository.DeleteRangeAsync(cart);
                    return Ok(new SharedResponse(true, 200, "Items Removed Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Item"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

    }
}
