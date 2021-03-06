using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.DataAccess.EntityFramework;
using Core.Extentions;
using Core.Paged;
using Core.Utilities.Results;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Models.Application;
using Models.Enums;
using Repository.Abstract;
using Repository.Helpers;
using ServiceStack;

namespace Repository.Concrete
{
    public class ApplicationRepository : EntityRepositoryBase<HealtyCareContext, Application>, IApplicationRepository
    {
        public PagedList<SickApplicationListModel> GetSickApplicationList(SickAplicationRequestModel model)
        {

            using (HealtyCareContext context = new HealtyCareContext())
            {
                var detailList = context.Applications
                    .Include(favoriteGenre => favoriteGenre.User)
                    .Where(x => x.User.UserType == 1 && x.Statu == model.Status);
                if (!string.IsNullOrEmpty(model.Filter))
                {
                    detailList = detailList.Where(x => x.User.FirstName.Contains(model.Filter) || x.User.LastName.Contains(model.Filter) || x.User.Mail.Contains(model.Filter) || x.User.Phone.Contains(model.Filter));
                }
                if (model.TransferType != 0)
                {
                    detailList = detailList.Where(x => x.TransferType == model.TransferType);
                }
                var list = detailList.Skip((model.Page - 1) * model.Limit).Take(model.Limit).Select(x => new SickApplicationListModel()
                {
                    Id = x.Id,
                    UserId = x.User.Id,
                    Mail = x.User.Mail,
                    Name = x.User.FirstName,
                    Surname = x.User.LastName,
                    Phone = x.User.Phone,
                    TransferType = x.TransferType,
                    TransferTypeString = ((TransferType)x.TransferType).GetDescription()

                }).ToList();

                var totalCount = context.Applications.Include(favoriteGenre => favoriteGenre.User).Count(x => x.User.UserType == 1 && x.Statu == model.Status);
                PagedList<SickApplicationListModel> donorListModel = new PagedList<SickApplicationListModel>();
                donorListModel.Items = list;
                donorListModel.PageSize = model.Limit;
                donorListModel.PageIndex = model.Page;
                donorListModel.TotalRecord = totalCount;
                donorListModel.TotalPage = totalCount / model.Limit;
                return donorListModel;
            }
        }
        public PagedList<DonorApplicationListModel> GetDonorApplicationList(DonorAplicationRequestModel model)
        {

            using (HealtyCareContext context = new HealtyCareContext())
            {

                var detailList = context.Applications
                    .Include(favoriteGenre => favoriteGenre.User)
                    .Where(x => x.User.UserType == 2 && x.Statu == model.Status);
                if (!string.IsNullOrEmpty(model.Filter))
                {
                    detailList = detailList.Where(x => x.User.FirstName.Contains(model.Filter) || x.User.LastName.Contains(model.Filter) || x.User.Mail.Contains(model.Filter) || x.User.Phone.Contains(model.Filter));
                }

                if (model.TransferType != 0)
                {
                    detailList = detailList.Where(x => x.TransferType == model.TransferType);
                }

                var list = detailList.Skip((model.Page - 1) * model.Limit).Take(model.Limit).Select(x => new DonorApplicationListModel()
                {
                    Id = x.Id,
                    UserId = x.User.Id,
                    Mail = x.User.Mail,
                    Name = x.User.FirstName,
                    Surname = x.User.LastName,
                    Phone = x.User.Phone,
                    TransferType = x.TransferType,
                    TransferTypeString = ((TransferType)x.TransferType).GetDescription()

                }).ToList();

                var totalCount = context.Applications.Include(favoriteGenre => favoriteGenre.User).Count(x => x.User.UserType == 2 && x.Statu == model.Status);
                PagedList<DonorApplicationListModel> donorListModel = new PagedList<DonorApplicationListModel>();
                donorListModel.Items = list;
                donorListModel.PageSize = model.Limit;
                donorListModel.PageIndex = model.Page;
                donorListModel.TotalRecord = totalCount;
                donorListModel.TotalPage = totalCount / model.Limit;
                return donorListModel;
            }
        }
        public List<Question> GetQuestionList()
        {

            using (HealtyCareContext context = new HealtyCareContext())
            {
                var questionList = context.Questions.Where(x => x.UserType == Convert.ToInt32(SessionHelper.DefaultSession.UserType))
                   .ToList();
                return questionList;
            }
        }
        public PagedList<UserApplicationModel> GetUserApplicationInformList(UserAplicationRequestModel model)

        {
            using (HealtyCareContext context = new HealtyCareContext())
            {
                var appList = context.Applications.Include(app => app.User)
                    .Where(x => x.UserId == Convert.ToInt32(SessionHelper.DefaultSession.Id) && x.Statu == model.Status);

                if (!string.IsNullOrEmpty(model.Filter))
                {
                    appList = appList.Where(x => x.RelativesName.Contains(model.Filter) || x.RelativeSurname.Contains(model.Filter) || x.RelativesPhone.Contains(model.Filter));
                }
                if (model.TransferType != 0)
                {
                    appList = appList.Where(x => x.TransferType == model.TransferType);
                }
                var list = appList.Skip((model.Page - 1) * model.Limit).Take(model.Limit).Select(m => new UserApplicationModel()
                {
                    Id = m.Id,
                    Name = m.User.FirstName,
                    Surname = m.User.LastName,
                    ApplicationDateTime = m.CreateDate,
                    Statu = m.Statu,
                    Description = m.Description,
                    RelativesName = m.RelativesName,
                    RelativesSurname = m.RelativeSurname,
                    RelativesPhone = m.RelativesPhone,
                    UpdateDateTime = m.UpdateDate,
                    TransferType = m.TransferType,
                    TransferTypeString = ((TransferType)m.TransferType).GetDescription()
                }).ToList();


                var totalCount = context.Applications.Include(x => x.User).Count(x => x.UserId == SessionHelper.DefaultSession.Id && x.Statu == model.Status);
                PagedList<UserApplicationModel> donorListModel = new PagedList<UserApplicationModel>();
                donorListModel.Items = list;
                donorListModel.PageSize = model.Limit;
                donorListModel.PageIndex = model.Page;
                donorListModel.TotalRecord = totalCount;
                donorListModel.TotalPage = totalCount / model.Limit;
                return donorListModel;

            }
        }
        public UserApplicationModel GetUserApplicationInform(int applicationId)
        {
            using (HealtyCareContext context = new HealtyCareContext())
            {
                return context.Applications.Include(app => app.User).Include(app => app.QuestionResult)
                    .Where(x => x.Id == applicationId)
                    .Select(m => new UserApplicationModel()
                    {
                        Id = m.Id,
                        Name = m.User.FirstName,
                        Surname = m.User.LastName,
                        ApplicationDateTime = m.CreateDate,
                        Statu = m.Statu,
                        Description = m.SickApplicationDetails.First().SicknessDetail,
                        RelativesName = m.RelativesName,
                        UpdateDateTime = m.UpdateDate,
                        TransferType = m.TransferType,
                        RelativesPhone = m.RelativesPhone,
                        RelativesSurname = m.RelativeSurname,
                        SicknessDate = m.SickApplicationDetails.First().SicknessDate,
                        SicknessDetailId = m.SickApplicationDetails.First().Id,
                        QuestionResulList = m.QuestionResult.ToList()
                    }).FirstOrDefault();
            }
        }
        public Application SetApplication(ApplicationSaveRequestModel model)
        {
            var list = new List<QuestionResult>();

            foreach (var item in model.QuestionResultList)
            {
                var question = new QuestionResult() { QuestionId = item.QuestionId, Result = item.QuestionResult, ApplicationId = model.Id };
                if (item.Id != 0)
                {
                    question.Id = item.Id;
                }
                list.Add(question);
            }

            var sicknessDetail =
                new SickApplicationDetails
                { SicknessDate = model.SickDate, SicknessDetail = model.SickDesc };
            if (model.SicknessDetailId != null)
            {
                sicknessDetail.Id = model.SicknessDetailId.Value;
            }

            var userApplication = new Application()
            {
                RelativesName = model.RelativesName,
                RelativeSurname = model.RelativesSurname,
                RelativesPhone = model.RelativesPhone,
                UserId = SessionHelper.DefaultSession.Id,
                TransferType = model.TransferType,
                Statu = 1,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                QuestionResult = list,
                SickApplicationDetails = new List<SickApplicationDetails>() { sicknessDetail },
                Report = new List<Report> { new Report { ReportName = model.ReportName } }
            };

            if (model.Id == null || model.Id == 0)
            {
                Add(userApplication);
            }
            else
            {
                userApplication.Id = model.Id.Value;
                Update(userApplication);
            }

            return userApplication;

        }
        public bool SetApplicationState(StateSaveRequestModel model)
        {
            using (HealtyCareContext context = new HealtyCareContext())
            {
                var app = Get(x => x.Id == model.AppId).FirstOrDefault();

                app.Id = model.AppId;
                app.CancellationReason = model.PlatformType == 0 ? "" : model.Description;
                app.Statu = model.PlatformType;
                app.UserId = model.UserId;

                var application = Update(app);
                context.SaveChanges();
                return application;
            }
        }
        public bool SetDonorUserMach(UserApplicationMatch model)
        {
            using (HealtyCareContext context = new HealtyCareContext())
            {
                var application = context.UserApplicationMatches.Add(model);
                var result = context.SaveChanges();
                return result > 0 ? true : false;
            }
        }
    }
}
