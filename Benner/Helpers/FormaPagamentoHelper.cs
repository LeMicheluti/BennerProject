using Benner.Models;
using System;

namespace Benner.Helpers
{
    public static class FormaPagamentoHelper
    {
        public static Array GetValues => Enum.GetValues(typeof(FormaPagamento));
    }
}
