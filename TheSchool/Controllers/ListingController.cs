using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheSchool.Entities;
using TheSchool.Entities.Export;
using TheSchool.Models;

namespace TheSchool.Controllers
{
    public class ListingController : Controller
    {
        Services.IQueryService<KnowledgeBaseItem> KnowledgeQuery;
        Services.IExportService<QnAMakerSetting> KnowledgeQnAExport;
        readonly AutoMapper.IMapper mapper;
        public ListingController(Services.IQueryService<KnowledgeBaseItem> queryService, Services.IExportService<QnAMakerSetting> exportService)
        {
            KnowledgeQuery = queryService;
            KnowledgeQnAExport = exportService;

            //TODO: Implement mapping from Entities.KnowledgeBaseItem to QuestionAndAnswerItemModel.
            //LastUpdateOn field is set with DateTime.Now and Tags field with lowercase.
            //Also create a map from TagItem to TagModel.
            var configurationManager = new AutoMapper.MapperConfiguration(
               cfg => {
                   cfg.CreateMap<KnowledgeBaseItem, QuestionAndAnswerItemModel>()
                       .ForMember(x => x.Question, opt => opt.MapFrom(z => z.Query))
                       .ForMember(x => x.LastUpdateOn, opt => opt.MapFrom(z => DateTime.Now))
                       .ForMember(x => x.Tags, opt => opt.MapFrom(z => z.Tags.ToLower()));

                   //cfg.CreateMap<TagItem, TagModel>();
               });
            mapper = configurationManager.CreateMapper();

        }

        [HttpGet]
        public ActionResult Index(string tag = "")
        {
            //TODO: Implement the corresponding call to get all items or filtered by tag.
            //Return an instance of ListingViewModel.
            var results = string.IsNullOrEmpty(tag)
                ? KnowledgeQuery.GetAll()
                : KnowledgeQuery.GetByFilter(x => x.Tags.Contains(tag));

            ListingViewModel model = new ListingViewModel()
            {
                Questions = mapper.Map < List<QuestionAndAnswerItemModel>>( results),
                Tag = tag
            };
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult ExportQnAMaker(string fileName, string folder)
        {
            var file = string.IsNullOrEmpty(fileName) ? System.Guid.NewGuid().ToString() + ".txt" : fileName;
            var path = string.IsNullOrEmpty(folder) ? AppDomain.CurrentDomain.BaseDirectory + @"\Export\" : folder;

            var knowledgeBase = KnowledgeQuery.GetAll();
            KnowledgeQnAExport.Export(knowledgeBase, new QnAMakerSetting(path, file));

            return File(path + file, "application/text", file);
        }
    }
}