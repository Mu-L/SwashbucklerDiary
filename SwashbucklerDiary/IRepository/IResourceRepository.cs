﻿using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IRepository
{
    public interface IResourceRepository : IBaseRepository<ResourceModel>
    {
        Task<List<ResourceModel>> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> func);
    }
}