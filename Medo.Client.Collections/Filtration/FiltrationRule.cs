using Medo.Core.Enums;
using Medo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.Collections.Filtration
{
    public class FiltrationRule : FiltrationRule<ActiveFilterEnum>
    {
        protected override string getFilterName()
        {
            switch (FilterType)
            {
                default: return null;
                case ActiveFilterEnum.Type:
                    {
                        return $"Вид документа: {(string)SearchingObject}";
                    }
                case ActiveFilterEnum.EODate:
                    {
                        return $"Дата опубликования: {((DateTime)SearchingObject).ToString("dd.MM.yyyy")}";
                    }
                case ActiveFilterEnum.Number:
                    {
                        return $"Номер документа: {(string)SearchingObject}";
                    }
                case ActiveFilterEnum.EONumber:
                    {
                        return $"Номер опубликования: {(string)SearchingObject}";
                    }
                case ActiveFilterEnum.Organ:
                    {
                        return $"Принявший орган: {(string)SearchingObject}";
                    }
                case ActiveFilterEnum.Signed:
                    {
                        return $"Дата подписания: {((DateTime)SearchingObject).ToString("dd.MM.yyyy")}";
                    }
                case ActiveFilterEnum.DeliveryTime:
                    {
                        return $"Дата поступления по МЭДО: {((DateTime)SearchingObject).ToString("dd.MM.yyyy")}";
                    }
                case ActiveFilterEnum.NotOpublic:
                    {
                        IsDefaultRule = true;
                        IsGenericRule = true;
                        RuleIsOn = true;
                        return "Неопубликованные документы";
                    }
                case ActiveFilterEnum.Changed:
                    {
                        IsGenericRule = true;
                        return "Заменненые документы";
                    }
                case ActiveFilterEnum.Deleted:
                    {
                        IsGenericRule = true;
                        return "Удаленные документы";
                    }
                case ActiveFilterEnum.Specialized:
                    {
                        RuleIsOn = true;
                        return "Просмотр дубликатов";
                    }
            }
        }
    }
}
