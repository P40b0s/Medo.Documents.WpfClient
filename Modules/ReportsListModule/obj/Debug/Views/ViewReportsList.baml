   M S B A M L   `   `   `  ����T  PMedo.Client.Notifications, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulltPclr-namespace:Medo.Client.Notifications.Views;assembly=Medo.Client.NotificationsMedo.Client.Notifications.Views  5       ReportsListModule:   4Medo.Modules.ReportsListModule.Views.ViewReportsList   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5      dnotificationsPclr-namespace:Medo.Client.Notifications.Views;assembly=Medo.Client.Notifications  5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � 	Resources 5	       �� MergedDictionaries �� Source�� 5
   
    5      �� 5      $T Npack://application:,,,/ReportsListModule;component/Templates/HeadersStyle.xaml?�6%   �� 5      $Z Tpack://application:,,,/ReportsListModule;component/Templates/ListViewItemsStyle.xaml?�6%   �� 5      $U Opack://application:,,,/ReportsListModule;component/Templates/CellsTemplate.xaml?�6%   �� 5      $N Hpack://application:,,,/ReportsListModule;component/Templates/Button.xaml?�6%   �� 5      $Z Tpack://application:,,,/ReportsListModule;component/Templates/AdressListTemplate.xaml?�6%   %    5      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   .+�5   
   ��  K� ZIndex$ 5��6      ListViewItemStyle#��[  5      $��0,0,0,40q�5       �� SelectedValuePath$ Value��5      $��0��5      $	��850��5       ��5      �� *ReportsCollection+ �� View 5"      �� 5#      .P�5$      �� $��
Выбор��6%    �� Width$ 60��5%   !    " ListViewHeaderStyleWithOutSort	 �� HeaderContainerStyle#	 [ 5&   !     ButtonCellTemplate
 �� CellTemplate#
 [ 5'   !   5(      �� 5+      $��
Орган��6%   $	 200��5,   !     OrganNameCellTemplate#
 [ 5-   !   5.      �� 50      $��Тип��6%   $	 100��51   !     ActTypeCellTemplate#
 [ 52   !   53      �� 55      $��
Номер��6%   $ 90��56   !     NumberCellTemplate#
 [ 57   !   58      �� 5:      $��Подписан��6%   $ 90��5;   !     SignDateCellTemplate#
 [ 5<   !   5=      �� 5?      $+��%Номер опубликования��6%   $	 200��5@   !     EoNumberCellTemplate#
 [ 5A   !   5B      �� 5D      $)��#Дата опубликования��6%     PublicationDateCellTemplate#
 [ 5E   !   5F      �� 5H      $!��Время отправки��6%    "	 AutoSendNotifyDateCellTemplate#
 [	 5I   !   5J      �� 5L      $#��Ручная отправка��6%   $	 150��5M   !    $
  ManualSendNotifyDateCellTemplate#
 [
 5N   !   5O      5P      5S      �� 5V   
   -   5         ��Name$ 
rightPanel��5V   &   $��1��6   .+�5W      �� $��0,10,0,0q�6    d� 	IsChecked 6)   �� *IsOldReport+ �� Mode$ TwoWay=�.��5X      ��  �� Orientation$ 
Horizontal=�6   .+�5Z      �� $$��Отчеты в Госдуму��6    $��5,0,0,0q�68   $��Center=�6I   5[      5\      &   System.Windows.Controls.Calendar  5^         SelectionMode$ MultipleRange=�6      SelectedDate 66   �� *SelectedDate+$ TwoWay=�   DisplayDate 6i   �� *SelectedDate+$ TwoWay=��� 5`      $��1��6   $��0,10,0,0q�6(   ��6:   �� *SendReportsCommand+.��5a      � $
��Left=�6    5b      �� 5c      $��40 �6+   �� 5d      $
��500* �6+   .+�5g      �� $-��'Отправка уведомлений��6    $��Center=�6<   $��1��6W   $��0,0,0,0q�6g   $��18(�6x   5h      5i      � 5j      .+�5k      � $��50��6   $��0,5,0,0q�6x    � Maximum 6*   �� *SelectedReportsCount+ � Value 6S   �� *SendedReportsCount+6x   �� 5l      $ 
Horizontal=�6   $��Center=�66   .+�5m      �� $��18(�6!   $��Center=�6s   ��6/   �� *SendedReportsCount+ �� StringFormat$ Обработано {0:}��6s   �� 5n      $��18(�6!   $��Center=�6m   $��5,0,0,0q�6�   ��6/   �� *SelectedReportsCount+$ 	из {0:}��6�   5o      5p      �� 5r      $��0 10 0 0q�6   $K��EВыбор адресатов отправки уведомлений��5s       � ToolTip$� �Если в списке присутсвуют адреса,
 то рассылка будет производиться по ним,
 если отсутсвуют то рассылка 
 производится по адресам назначенным по умолчанию��5u      ��5t      �� *SelectAdressCommand+5u      �� 5v       �� DisplayMemberPath$ Adress��6     adressListTemplate#��[ 5w      ��5x      �� *NotificationAdressList+5{      .  (System.Windows.Interactivity.Interaction   Triggers 5�   
   F  @Prism.Interactivity.InteractionRequest.InteractionRequestTrigger  5�      3  -System.Windows.Interactivity.EventTriggerBase   SourceObject 6.   �� *SelectAdressRequest+$ OneWay=�.  (System.Windows.Interactivity.TriggerBase   Actions. 5�      +  %Prism.Interactivity.PopupWindowAction     IsModal .    WindowStartupLocation$ CenterScreenX�69      WindowStyle 5�      �� 5�       �� 
TargetType" �6    .�5�      ��  �� Property �@�SizeToContent �� Value$ WidthAndHeight=�6>   �� 5�       �@�WindowStyle$
 None=�6<   �� 5�       �@�
ResizeMode$ NoResize=�6;   �� 5�       �@�ShowInTaskbar .  6>   �� 5�       �@�AllowsTransparency . 6C   �� 5�       � ��
 ���� 6;   5�          WindowContent  5�      <   6Medo.Client.Notifications.Views.SelectAdressesListView  5�      5�      5�      5�      5�      