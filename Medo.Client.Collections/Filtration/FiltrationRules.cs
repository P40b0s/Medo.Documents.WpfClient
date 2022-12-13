using Medo.Core.Collections;
using Medo.Core.Enums;
using Medo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.Collections.Filtration
{
    /// <summary>
    /// Коллекция фильтров класс - Document, тип фильтрации - ActiveFilterEnum
    /// </summary>
    public class FiltrationRules : FiltrationRules<Document, ActiveFilterEnum>
    {
        /// <summary>
        /// установка фильтрации используемой по умолчанию
        /// </summary>
        /// <param name="rule"></param>
        public FiltrationRules(ActiveFilterEnum rule, IComparer<Document> defaultComparer = null) : base(rule, defaultComparer) {  }
        public FiltrationRules() { }
        protected async override Task<List<Predicate<Document>>> FilterItem(Document filteredItem)
        {
            List<Predicate<Document>> FilterCriteria = new List<Predicate<Document>>();
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var items = this.Where(i => i.Value.RuleIsOn);
                    foreach (var item in items)
                    {
                        switch (item.Key)
                        {
                            case Core.Enums.ActiveFilterEnum.Organ:
                                {
                                    if (item.Value.SearchingObject != null)
                                    {
                                        FilterCriteria.Add(new Predicate<Document>(d => d.OrganName == (string)item.Value.SearchingObject));
                                    }
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.Type:
                                {
                                    if (item.Value.SearchingObject != null)
                                    {
                                        FilterCriteria.Add(new Predicate<Document>(d => d.ActType == (string)item.Value.SearchingObject));
                                    }
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.Number:
                                {
                                    if (item.Value.SearchingObject != null)
                                    {
                                        FilterCriteria.Add(new Predicate<Document>(d =>
                                                          (d.DocumentNumber != null ? d.DocumentNumber.ToLower().Contains(((string)item.Value.SearchingObject).ToLower()) : false)
                                                       || (d.ChangedNumber != null ? d.ChangedNumber.ToLower().Contains(((string)item.Value.SearchingObject).ToLower()) : false)
                                                       || (d.MJNumber != null ? d.MJNumber.Contains((string)item.Value.SearchingObject) : false)));
                                    }
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.EONumber:
                                {
                                    if (item.Value.SearchingObject != null)
                                    {
                                        FilterCriteria.Add(new Predicate<Document>(d =>
                                                          (d.EoNumber == (string)item.Value.SearchingObject)
                                                        || (d.EoNumber != null && d.EoNumber.Substring(d.EoNumber.Length - 4) == (string)item.Value.SearchingObject)));
                                    }
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.Signed:
                                {
                                    if (item.Value.SearchingObject != null)
                                    {
                                        FilterCriteria.Add(new Predicate<Document>(d =>
                                                          (d.SignDate.HasValue && d.SignDate.Value.Date == ((DateTime?)item.Value.SearchingObject).Value.Date)
                                                       || (d.MJDate.HasValue && d.MJDate.Value.Date == ((DateTime?)item.Value.SearchingObject).Value.Date)));
                                    }
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.EODate:
                                {
                                    if (item.Value.SearchingObject != null)
                                    {
                                        FilterCriteria.Add(new Predicate<Document>(d =>
                                                          (d.PublDatePortal.HasValue
                                                        && d.PublDatePortal.Value.Date == ((DateTime?)item.Value.SearchingObject).Value.Date)));
                                    }
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.DeliveryTime:
                                {
                                    if (item.Value.SearchingObject != null)
                                    {
                                        FilterCriteria.Add(new Predicate<Document>(d =>
                                                          (d.DeliveryTime.HasValue
                                                        && d.DeliveryTime.Value.Date == ((DateTime?)item.Value.SearchingObject).Value.Date)));
                                    }
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.NotOpublic:
                                {
                                    FilterCriteria.Add(new Predicate<Document>(d =>
                                          (d.IsInvisible == false)
                                       && (d.IzdanieDocumentStatus != new Guid("9F00E217-DB46-4CAC-9FCB-EE148DB34C5F"))
                                       && (d.IsChanged == false)
                                       && (d.NotificationStatus == 0 || d.NotificationStatus == 1 || d.NotificationStatus == 100)));
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.Changed:
                                {
                                    FilterCriteria.Add(new Predicate<Document>(d =>
                                        (d.IsChanged == true)));
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.Deleted:
                                {
                                    FilterCriteria.Add(new Predicate<Document>(d =>
                                          (d.IsInvisible == true)));
                                    break;
                                }
                            case Core.Enums.ActiveFilterEnum.Specialized:
                                {
                                    FilterCriteria.Add(new Predicate<Document>(d =>
                                          (SpecializedFiltrationList.Contains(d.HeaderGuid))));
                                    break;
                                }
                        }
                    }
                    return FilterCriteria;
                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                    return FilterCriteria;
                }
            });
        }

        public async override void SourceCollectionFiltration(IList<Document> sourceCollection)
        {
            if (sourceCollection != null)
            {
                FilteredItems.Clear();
                ActivateAllRules();
                for (int i = 0; i < sourceCollection.Count; i++)
                {
                    bool f = await IsPassedFilter(sourceCollection[i]);
                    sourceCollection[i].Filter = f;
                    if (f)
                    {
                        FilteredItems.AddSorted(sourceCollection[i]);
                    }
                }
                OnPropertyChanged("FilteredItemsCount");
                OnPropertyChanged("ActiveFiltersCount");
                OnPropertyChanged("ActiveRulesCount");
            }
        }
        /// <summary>
        /// Добавление, обновление или удаление файлов из привязанной коллекции
        /// </summary>
        /// <param name="doc"></param>
        public async override void OneElementFiltration(Document item)
        {
            try
            {
                item.Filter = await IsPassedFilter(item);
                if (item.Filter)
                {
                    
                    if (!FilteredItems.Contains(item))
                    {
                        item.daysLeft = new DaysLeft(item);
                        FilteredItems.AddSorted(item);
                    }
                    else
                    {
                        item.daysLeft = new DaysLeft(item);
                    }
                }
                else
                {
                    FilteredItems.Remove(item);
                }
                OnPropertyChanged("FilteredItemsCount");
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
