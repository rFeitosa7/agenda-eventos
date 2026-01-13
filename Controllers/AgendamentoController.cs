using Agendamento_de_Eventos.Data;
using Agendamento_de_Eventos.Enums;
using Agendamento_de_Eventos.Models;
using Agendamento_de_Eventos.Repositorio;
using Agendamento_de_Eventos.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Agendamento_de_Eventos.Controllers
{
    public class AgendamentoController : Controller
    {
        private readonly InterfaceAgendamento _interface;

        public AgendamentoController (InterfaceAgendamento interfaceAgendamento)
        {
            _interface = interfaceAgendamento;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddEvento (AgendamentoViewModel agendamento)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErroDataModel"] = "Erro ao agendar o evento. Tente Novamente!";
                return View("Index", agendamento);
            }

            int ano = DateTime.Now.Year;

            DateTime dataEvento;

            try
            {
                  dataEvento = new DateTime(
                    ano,
                    (int)agendamento.MesEnum,
                    agendamento.Dia.Value
                  );
            }
            catch
            {
                ModelState.AddModelError("", "Data Inválida.");
                TempData["ErroDataInvalida"] = "Data Inválida. Digite Novamente!";
                return View("Index", agendamento);
            }

                var agendamentoModel = new AgendamentoModel
                {
                    Nome = agendamento.Nome,
                    DataEvento = dataEvento,
                    Duracao = (DuracaoEvento)agendamento.DuracaoEvento,
                    TipoEvento = (TipoEvento)agendamento.TipoEvento,
                    NumeroCelular = agendamento.NumeroCelular,
                    Localizacao = agendamento.Localizacao,
                    InfoEvento = agendamento.InfoEvento,
                    Horainicio = (TimeSpan)agendamento.Horainicio,
                };

            _interface.AddBanco(agendamentoModel);

            string PrecoeHora = string.Empty;

            switch (agendamento.DuracaoEvento)
            {
                case DuracaoEvento.UmaHora:
                    PrecoeHora =  "Duração e Valor contratado : 1 Horas - R$ 300,00";
                    break;

                case DuracaoEvento.DuasHora:
                    PrecoeHora = "Duração e Valor contratado : 2 Horas - R$ 500,00";
                    break;

                case DuracaoEvento.TresHora:
                    PrecoeHora = "Duração e Valor contratado : 3 Horas - R$ 700,00";
                    break;

                case DuracaoEvento.QuatroHora:
                    PrecoeHora = "Duração e Valor contratado : 4 Horas - R$ 900,00";
                    break;
            };
           

            string mensagemWhatsapp =
                $"Olá, gostaria de confirmar o agendamento.\n\n" +
                $"Nome : {agendamentoModel.Nome}\n" +
                $"Data Evento : {agendamentoModel.DataEvento}\n" +
                $"{PrecoeHora}\n"+
                $"Horário de inicio do evento : {agendamentoModel.Horainicio}\n"+
                $"Tipo de Evento : {agendamentoModel.TipoEvento}\n" +
                $"Localização : {agendamentoModel.Localizacao}\n" +
                $"Observações {agendamentoModel.InfoEvento}\n\n" +
                $"Chave PIX : 69992955498 - Nubank - Jucimar Moraes Rodrigues Queiroz";

                

            TempData["MensagemURL"] = Uri.EscapeDataString(mensagemWhatsapp);

            TempData["SucecessAgendamento"] = "Agendamento enviado com Sucesso!!";
            return View("SucessoAgenda");
        }
    }
}
