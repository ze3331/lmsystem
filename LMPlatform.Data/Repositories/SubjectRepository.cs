﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Application.Core.Data;
using LMPlatform.Data.Infrastructure;
using LMPlatform.Data.Repositories.RepositoryContracts;
using LMPlatform.Models;

namespace LMPlatform.Data.Repositories
{
    using System.IO;

    using Newtonsoft.Json;

    public class SubjectRepository : RepositoryBase<LmPlatformModelsContext, Subject>, ISubjectRepository
	{
		public SubjectRepository(LmPlatformModelsContext dataContext)
			: base(dataContext)
		{
		}

		public List<Subject> GetSubjects(int groupId = 0, int lecturerId = 0)
		{
			using (var context = new LmPlatformModelsContext())
			{
				if (groupId != 0)
				{
					var subjectGroup = context.Set<SubjectGroup>().Include(e => e.Subject.SubjectGroups.Select(x => x.SubjectStudents))
                            .Where(e => e.GroupId == groupId).ToList();
					return subjectGroup.Select(e => e.Subject).ToList();
				}
				else
				{
					var subjectLecturer =
                        context.Set<SubjectLecturer>()
                        .Include(e => e.Subject.SubjectGroups.Select(x => x.SubjectStudents))
                        .Where(e => e.LecturerId == lecturerId).ToList();
					return subjectLecturer.Select(e => e.Subject).ToList();
				}
			}
		}

		public SubjectNews SaveNews(SubjectNews news)
		{
			using (var context = new LmPlatformModelsContext())
			{
				if (news.Id != 0)
				{
					context.Update(news);
				}
				else
				{
					context.Add(news);
				}

				context.SaveChanges();
			}

			return news;
		}

		public void DeleteNews(SubjectNews news)
		{
			using (var context = new LmPlatformModelsContext())
			{
				var model = context.Set<SubjectNews>().FirstOrDefault(e => e.Id == news.Id);
				context.Delete(model);

				context.SaveChanges();
			}
		}

	    public void DeleteLection(Lectures lectures)
	    {
            using (var context = new LmPlatformModelsContext())
            {
                var model = context.Set<Lectures>().FirstOrDefault(e => e.Id == lectures.Id);
                
                context.Delete(model);

                context.SaveChanges();
            }
	    }

	    protected override void PerformAdd(Subject model, LmPlatformModelsContext dataContext)
		{
			base.PerformAdd(model, dataContext);

			foreach (var subjectModule in model.SubjectModules)
			{
				subjectModule.SubjectId = model.Id;
				dataContext.Set<SubjectModule>().Add(subjectModule);
			}

			foreach (var subjectGroup in model.SubjectGroups)
			{
				subjectGroup.SubjectId = model.Id;
				dataContext.Set<SubjectGroup>().Add(subjectGroup);
			}

			model.SubjectLecturers.FirstOrDefault().SubjectId = model.Id;
			dataContext.Set<SubjectLecturer>().Add(model.SubjectLecturers.FirstOrDefault());

			dataContext.SaveChanges();
		}

		protected override void PerformUpdate(Subject newValue, LmPlatformModelsContext dataContext)
		{
			var subjectModules = dataContext.Set<SubjectModule>().Where(e => e.SubjectId == newValue.Id).ToList();

			foreach (var subjectModule in subjectModules)
			{
				if (!newValue.SubjectModules.Any(e => e.ModuleId == subjectModule.ModuleId))
				{
					dataContext.Set<SubjectModule>().Remove(subjectModule);
				}
			}

			foreach (var subjectModule in newValue.SubjectModules)
			{
				if (!subjectModules.Any(e => e.ModuleId == subjectModule.ModuleId))
				{
					dataContext.Set<SubjectModule>().Add(subjectModule);
				}
			}

			var subjectGroups = dataContext.Set<SubjectGroup>().Where(e => e.SubjectId == newValue.Id).ToList();

			foreach (var subjectGroup in subjectGroups)
			{
				if (!newValue.SubjectGroups.Any(e => e.GroupId == subjectGroup.GroupId))
				{
					dataContext.Set<SubjectGroup>().Remove(subjectGroup);
				}
			}

			foreach (var subjectGroup in newValue.SubjectGroups)
			{
				if (!subjectGroups.Any(e => e.GroupId == subjectGroup.GroupId))
				{
					dataContext.Set<SubjectGroup>().Add(subjectGroup);
				}
			}

			dataContext.SaveChanges();

			base.PerformUpdate(newValue, dataContext);
		}
	}
}