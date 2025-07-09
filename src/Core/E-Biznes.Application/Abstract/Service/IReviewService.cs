using E_Biznes.Application.DTOs.ReviewDtos;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface IReviewService
{
    Task<BaseResponse<string>> CreateReviewAsync(Guid productId, ReviewCreateDto dto);
    Task<BaseResponse<string>> DeleteAsync(Guid id);
    Task<BaseResponse<List<ReviewGetDto>>> GetReviewByProductIdAsync(Guid productId);
    Task<BaseResponse<List<ReviewUserGetDto>>> GetReviewsByUserIdAsync(string userId);
}
