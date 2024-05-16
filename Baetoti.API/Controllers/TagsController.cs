using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Request.TagRequest;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.TagResponse;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class TagsController : ApiBaseController
    {
        public readonly ITagsRepository _tagsRepository;
        public readonly IMapper _mapper;

        public TagsController(
           ITagsRepository tagsRepository,
           IMapper mapper
           )
        {
            _tagsRepository = tagsRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tagList = await _tagsRepository.GetAllTagsAsync();
                return Ok(new SharedResponse(true, 200, "", tagList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

		[HttpGet("GetAllStoreTags")]
		public async Task<IActionResult> GetAllStoreTags()
		{
			try
			{
				var tagList = await _tagsRepository.GetAllStoreTags();
				return Ok(new SharedResponse(true, 200, "", tagList));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message, null));
			}
		}

		[HttpGet("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            try
            {
                var tag = await _tagsRepository.GetTagByIDAsync(Id);
                return Ok(new SharedResponse(true, 200, "", tag));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetBySubCategoryId")]
        public async Task<IActionResult> GetBySubCategoryId(int subCategoryID)
        {
            try
            {
                var tags = await _tagsRepository.GetTagBySubCategoryIDAsync(subCategoryID);
                return Ok(new SharedResponse(true, 200, "", tags));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] TagRequest tagRequest)
        {
            try
            {
                var tag = _mapper.Map<Tags>(tagRequest);
                tag.MarkAsDeleted = false;
                tag.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                tag.CreatedBy = Convert.ToInt32(UserId);
                tag.TagStatus = (int)TagStatus.Active;
                var result = await _tagsRepository.AddAsync(tag);
                if (result == null)
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Create Tag"));
                }
                return Ok(new SharedResponse(true, 200, "Tag Created Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] TagRequest tagRequest)
        {
            try
            {
                var tag = await _tagsRepository.GetByIdAsync(tagRequest.ID);
                if (tag != null)
                {
                    tag.TagType = tagRequest.TagType;
                    tag.CategoryID = tagRequest.CategoryID;
                    tag.SubCategoryID = tagRequest.SubCategoryID;
                    tag.TagEnglish = tagRequest.TagEnglish;
                    tag.TagArabic = tagRequest.TagArabic;
                    tag.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    tag.UpdatedBy = Convert.ToInt32(UserId);
                    await _tagsRepository.UpdateAsync(tag);
                    return Ok(new SharedResponse(true, 200, "Tag Updated Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Tag"));
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
                var tag = await _tagsRepository.GetByIdAsync(request.ID);
                if (tag != null)
                {
                    tag.TagStatus = request.Value;
                    tag.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    tag.UpdatedBy = Convert.ToInt32(UserId);
                    await _tagsRepository.UpdateAsync(tag);
                    return Ok(new SharedResponse(true, 200, "Tag UnSuspended Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Tag"));
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
                var tag = await _tagsRepository.GetByIdAsync(ID);
                if (tag != null)
                {
                    tag.MarkAsDeleted = true;
                    tag.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    tag.CreatedBy = Convert.ToInt32(UserId);
                    await _tagsRepository.UpdateAsync(tag);
                    return Ok(new SharedResponse(true, 200, "Tag Deleted Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Tag"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }
    }
}
