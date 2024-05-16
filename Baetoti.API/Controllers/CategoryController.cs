using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.API.Helpers;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Category;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Category;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
	public class CategoryController : ApiBaseController
	{

		public readonly ICategoryRepository _categoryRepository;
		public readonly IMapper _mapper;

		public CategoryController(
			ICategoryRepository categoryRepository,
			IMapper mapper
			)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		[AllowAnonymous]
		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var categoryList = (await _categoryRepository.ListAllAsync()).
					Where(x => x.MarkAsDeleted == false).ToList();
				return Ok(new SharedResponse(true, 200, "", _mapper.Map<List<CategoryResponse>>(categoryList)));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message, null));
			}
		}

		[AllowAnonymous]
		[HttpGet("GetAllByStoreID")]
		public async Task<IActionResult> GetAllByStoreID(long StoreID)
		{
			try
			{
				var categoryList = await _categoryRepository.GetAllByStoreID(StoreID);
				return Ok(new SharedResponse(true, 200, "", categoryList));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message, null));
			}
		}

        [AllowAnonymous]
        [HttpGet("GetById")]
		public async Task<IActionResult> GetById(int Id)
		{
			try
			{
				var category = await _categoryRepository.GetByIdAsync(Id);
				return Ok(new SharedResponse(true, 200, "", _mapper.Map<CategoryResponse>(category)));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message, null));
			}
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add([FromBody] CategoryRequest categoryRequest)
		{
			try
			{
				var category = _mapper.Map<Category>(categoryRequest);
				category.MarkAsDeleted = false;
				category.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
				category.CreatedBy = Convert.ToInt32(UserId);
				category.CategoryStatus = (int)CategoryStatus.Active;
				var result = await _categoryRepository.AddAsync(category);
				if (result == null)
				{
					return Ok(new SharedResponse(false, 400, "Unable To Create Category"));
				}
				return Ok(new SharedResponse(true, 200, "Category Created Succesfully"));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}

		[HttpPost("Update")]
		public async Task<IActionResult> Update([FromBody] CategoryRequest categoryRequest)
		{
			try
			{
				var cat = await _categoryRepository.GetByIdAsync(categoryRequest.ID);
				if (cat != null)
				{
					cat.CategoryName = categoryRequest.CategoryName;
					cat.CategoryArabicName = categoryRequest.CategoryArabicName;
					cat.Color = categoryRequest.Color;
					cat.Description = categoryRequest.Description;
					if (!string.IsNullOrEmpty(categoryRequest.Picture))
						cat.Picture = categoryRequest.Picture;
					cat.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
					cat.UpdatedBy = Convert.ToInt32(UserId);
					await _categoryRepository.UpdateAsync(cat);
					return Ok(new SharedResponse(true, 200, "Category Updated Succesfully"));
				}
				else
				{
					return Ok(new SharedResponse(false, 400, "unable to find category"));
				}
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}

		[HttpPost("UpdateStatus")]
		public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
		{
			try
			{
				var cat = await _categoryRepository.GetByIdAsync(request.ID);
				if (cat != null)
				{
					cat.CategoryStatus = request.Value;
					cat.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
					cat.UpdatedBy = Convert.ToInt32(UserId);
					await _categoryRepository.UpdateAsync(cat);
					return Ok(new SharedResponse(true, 200, "Category Status Updated Succesfully"));
				}
				else
				{
					return Ok(new SharedResponse(false, 400, "unable to find category"));
				}
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}

		[HttpDelete("Delete/{ID}")]
		public async Task<IActionResult> Delete(long ID)
		{
			try
			{
				var cat = await _categoryRepository.GetByIdAsync(ID);
				if (cat != null)
				{
					cat.MarkAsDeleted = true;
					cat.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
					cat.UpdatedBy = Convert.ToInt32(UserId);
					await _categoryRepository.UpdateAsync(cat);
					return Ok(new SharedResponse(true, 200, "Category Deleted Succesfully"));
				}
				else
				{
					return Ok(new SharedResponse(false, 400, "Unable To Find Category"));
				}
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}

		[HttpPost]
		[Route("UploadFile")]
		public async Task<IActionResult> UploadFile(IFormFile file)
		{
			try
			{
				if (file.Length > 0)
				{
					UploadImage obj = new UploadImage();
					FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, "Category");
					if (string.IsNullOrEmpty(_RESPONSE.Message))
					{
						return Ok(new SharedResponse(true, 200, "File uploaded successfully!", _RESPONSE));
					}
					else
					{
						return Ok(new SharedResponse(true, 400, _RESPONSE.Message));
					}
				}
				else
				{
					return Ok(new SharedResponse(false, 400, "File is required!"));
				}
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}
	}
}
