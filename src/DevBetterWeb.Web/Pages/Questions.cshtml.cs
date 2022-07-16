using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages;

public class QuestionsModel : PageModel
{
	private readonly IMapper _mapper;
	private readonly IRepository<Question> _questionRepository;

	public QuestionsModel(IMapper mapper, IRepository<Question> questionRepository)
	{
		_mapper = mapper;
		_questionRepository = questionRepository;
	}

  public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();

  public async Task OnGetAsync()
  {
	  var spec = new QuestionsSortedSpec();
	  var questionsEntity = await _questionRepository.ListAsync(spec);
	  Questions = _mapper.Map<List<QuestionDto>>(questionsEntity);
  }
}
