using System.Threading.Tasks;
using GrammarGuide.Services.Entities;

namespace GrammarGuide.Services.Services;

public interface IUserService
{
    Task<User> CreateUser(string userName);
    Task<GuideProgress> UpdateUnitGroupCompletion(string guideId, string userId, int unitGroupIndex, bool isCompleted);
    Task<GuideProgress> UpdateUnitCompletion(string guideId, string userId, int unitIndex, bool isCompleted);
    Task<User> Update(string userId, string theme, string name);
    Task<GuideProgress> UpdateBookmark(string guideId, string userId, int unitIndex, bool isAdded);
    Task<GuideProgress> UpdateExerciseCompletion(string guideId, string userId, int unitIndex, int exerciseIndex, bool isAdded);
    Task<User> GetUserProgress(string userId);
}