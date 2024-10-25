using System;
using System.Linq;
using System.Threading.Tasks;
using GrammarGuide.Services.Entities;
using MongoDB.Driver;

namespace GrammarGuide.Services.Services;

public class UserService(
    string userCollectionName,
    IMongoDatabase database) : IUserService
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>(userCollectionName);

    public async Task<User> CreateUser(string userName)
    {
        var user = new User
        {
            Name = userName,
            Theme = "Light",
            Progresses = [],
            Balance = 10
        };
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<GuideProgress> UpdateUnitGroupCompletion(string guideId, string userId, int unitGroupIndex,
        bool isCompleted)
    {
        var user = await GetUserProgress(userId);
        
        var progress = GetUserGuideProgress(user, guideId);

        if (isCompleted)
        {
            if (!progress.CompletedUnitGroupsIndexes.Contains(unitGroupIndex))
                progress.CompletedUnitGroupsIndexes.Add(unitGroupIndex);
        }
        else
        {
            if (progress.CompletedUnitGroupsIndexes.Contains(unitGroupIndex))
                progress.CompletedUnitGroupsIndexes.Remove(unitGroupIndex);
        }
        
        await _users.ReplaceOneAsync(g => user.Id == g.Id, user);
        return progress;
    }

    public async Task<GuideProgress> UpdateUnitCompletion(string guideId, string userId, int unitIndex,
        bool isCompleted)
    {
        var user = await GetUserProgress(userId);
        
        var progress = GetUserGuideProgress(user, guideId);

        if (isCompleted)
        {
            if (!progress.CompletedUnitIndexes.Contains(unitIndex))
                progress.CompletedUnitIndexes.Add(unitIndex);
        }
        else
        {
            if (progress.CompletedUnitIndexes.Contains(unitIndex))
                progress.CompletedUnitIndexes.Remove(unitIndex);
        }

        await _users.ReplaceOneAsync(g => user.Id == g.Id, user);
        return progress;
     }

    public async Task<User> Update(string userId, string theme, string name)
    {
        var user = await GetUserProgress(userId);
        user.Theme = theme;
        await _users.ReplaceOneAsync(g => user.Id == g.Id, user);
        return user;
    }

    public async Task<GuideProgress> UpdateBookmark(string guideId, string userId, int unitIndex, bool isAdded)
    {
        var user = await GetUserProgress(userId);
        
        var progress = GetUserGuideProgress(user, guideId);
        
        if (isAdded)
        {
            if (!progress.Bookmarks.Contains(unitIndex))
                progress.Bookmarks.Add(unitIndex);
        }
        else
        {
            if (progress.Bookmarks.Contains(unitIndex))
                progress.Bookmarks.Remove(unitIndex);
        }

        await _users.ReplaceOneAsync(g => user.Id == g.Id, user);
        return progress;
    }

    public async Task<GuideProgress> UpdateExerciseCompletion(string guideId, string userId, int unitIndex,
        int exerciseIndex, bool isAdded)
    {
        var user = await GetUserProgress(userId);

        var progress = GetUserGuideProgress(user, guideId);

        var exerciseProgress = progress.CompletedExercises.FirstOrDefault(c => c.UnitIndex == unitIndex);
        if (exerciseProgress == null)
        {
            exerciseProgress = new UnitProgress
            {
                UnitIndex = unitIndex,
                CompletedExercises = []
            };
            progress.CompletedExercises.Add(exerciseProgress);
        }

        if (isAdded)
        {
            exerciseProgress.CompletedExercises.Add(exerciseIndex);
        }
        else
        {
            exerciseProgress.CompletedExercises.Remove(exerciseIndex);
        }

        await _users.ReplaceOneAsync(g => user.Id == g.Id, user);
        return progress;
    }


    public async Task<User> GetUserProgress(string id)
    {
        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (user == null)
            throw new Exception("User not found");
        return user;
    }

    private GuideProgress GetUserGuideProgress(User user, string guideId)
    {
        var progressGuide = user.Progresses.FirstOrDefault(p => p.GuideId == guideId);
        if (progressGuide == null)
        {
            progressGuide = new GuideProgress
            {
                GuideId = guideId,
                CompletedUnitIndexes = [],
                CompletedUnitGroupsIndexes = [],
                Bookmarks = [],
                CompletedExercises = []
            };
            user.Progresses.Add(progressGuide);
        }

        return progressGuide;
    }
}

