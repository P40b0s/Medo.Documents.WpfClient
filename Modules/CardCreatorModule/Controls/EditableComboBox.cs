using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Medo.Modules.CardCreatorModule.Controls
{
    public class EditableComboBox : ComboBox
    {
        public delegate void TextChangedEventHandler(string changedtext);
        public EditableComboBox()
        {
            this.IsEditable = true;
            this.IsTextSearchEnabled = false;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (ItemsSource != null)
            {
               // collection = CollectionViewSource.GetDefaultView(this.ItemsSource);
               // collection.Filter = filter;
            }
            base.OnItemsSourceChanged(oldValue, newValue);
        }
        protected TextBox EditableTextBox
        {
            get
            {
                return this.GetTemplateChild("PART_EditableTextBox") as TextBox;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.IsDropDownOpen = true;
            EditableTextBox.Text = "";
            this.SelectedIndex = -1;
        }
        protected override void OnGotFocus(System.Windows.RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.IsDropDownOpen = true;
            EditableTextBox.Text = "";
            this.SelectedIndex = -1;

        }

        public void increment(string searchtext, List<string> BindingList, List<string> EtalonList)
        {
            if (searchtext.Length >= 0 && BindingList != null && EtalonList != null)
            {
                BindingList.Clear();
                for (int i = 0; i < EtalonList.Count; i++)
                {
                    BindingList.Add(EtalonList[i]);
                }
                for (int i = BindingList.Count - 1; i >= 0; i--)
                {
                    if (!BindingList[i].ToLower().Contains(searchtext.ToLower()))
                    {
                        BindingList.RemoveAt(i);
                    }
                    else
                    {
                        try
                        {
                            if ((BindingList[i].ToLower().IndexOf(searchtext)) != -1)
                            {
                                int t = (BindingList[i].ToLower().IndexOf(searchtext));
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            else
            {
                if (EtalonList != null)
                {
                    BindingList = new List<string>(EtalonList);
                }

            }
        }

        private bool filter(object data)
        {
            try
            {
                if (EditableTextBox != null)
                {
                    string text = EditableTextBox.Text;
                    string s = (string)data;
                    return s.ToLower().Contains(text.ToLower());
                }
                else
                {
                    return false;
                }
             
            }
            catch (Exception ex) { return true; };

        }
        private ICollectionView collection { get; set; }

        //protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        //{
        //    base.OnSelectionChanged(e);
        //    if (e.AddedItems.Count == 1 )
        //    {
        //        EditableTextBox.Text = (string)e.AddedItems[0];
        //    }
        //}
        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            if (EditableTextBox.Text.Length == 0)
            {
                collection.MoveCurrentToPosition(-1);
                collection.Filter = null;
            }
            this.IsDropDownOpen = true;
            if (collection.Filter == null)
            {
                collection.Filter = filter;
            }
            //if (collection.Cast<string>().Count() == 1)
            //{
            //    collection.MoveCurrentTo(collection.Cast<string>().FirstOrDefault());
            //    this.IsDropDownOpen = false;
            //    Keyboard.ClearFocus();
            //}
            collection.Refresh();
            //base.OnKeyUp(e);            
        }


    }
}
