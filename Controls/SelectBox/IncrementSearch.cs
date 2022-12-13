using Medo.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.SelectBox
{
    class IncrementSearch
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public void Search(string searchtext, ref IEnumerable<TextInlineSelection> collection)
        {
            try
            {
                if (searchtext != null && searchtext.Length >= 0)
                {

                    for (int i = 0; i < collection.Count(); i++)
                    {
                        collection.ElementAt(i).Visible = true;
                        collection.ElementAt(i).SelectedText = null;
                        collection.ElementAt(i).TextBeforeSelect = null;
                    }
                    for (int i = collection.Count() - 1; i >= 0; i--)
                    {
                        var item = collection.ElementAt(i) as TextInlineSelection;
                        if (item.SourceText != null)
                        {
                            if (!item.SourceText.ToLower().Contains(searchtext.ToLower()))
                            {
                                collection.ElementAt(i).Visible = false;
                            }
                            else
                            {
                                if ((item.SourceText.ToLower().IndexOf(searchtext.ToLower())) != -1)
                                {
                                    int t = (item.SourceText.ToLower().IndexOf(searchtext.ToLower()));
                                    item.TextBeforeSelect = item.SourceText.Substring(0, t);
                                    item.SelectedText = item.SourceText.Substring(item.SourceText.ToLower().IndexOf(searchtext.ToLower()), searchtext.Length);
                                }

                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < collection.Count(); i++)
                    {
                        collection.ElementAt(i).Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
