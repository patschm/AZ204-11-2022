using ACME.Business.Interfaces;
using ACME.UI.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Threading.Tasks;

namespace ACME.UI.Web.Controllers
{
    public class CalculatorController : Controller
    {
        private IFeatureManager _features;
        private ICalculator _calculator;

        public CalculatorController( ICalculator calculator)
        {
            _calculator = calculator;
        }

        public IActionResult Index(CalculatorModel? model = null)
        {
            if (model == null) model = new CalculatorModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(CalculatorModel model)
        {
            model.Result = _calculator.Add(model.A, model.B);
            return View("Index", model);
        }

        public async Task<IActionResult> Help()
        {
            return View();
        }
    }
}
