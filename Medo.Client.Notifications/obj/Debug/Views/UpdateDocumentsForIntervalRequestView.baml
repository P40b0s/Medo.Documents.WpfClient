   M S B A M L   `   `   `  ����  Medo.Client.NotificationsK    EMedo.Client.Notifications.Views.UpdateDocumentsForIntervalRequestView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $f `pack://application:,,,/Medo.Client.Notifications;component/Templates/ReportsSenderListStyle.xaml?�6%   �� 5      $W Qpack://application:,,,/Medo.Client.Notifications;component/Templates/Buttons.xaml?�6%   �� 5      $U Opack://application:,,,/Medo.ImageResources;component/PNGResourceDictionary.xaml?�6%   %    5      .��5      � $
��auto��6   $
��auto��6   
������� K� ZIndex$ 1��69    � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $
��auto �6   �� 5      $��40 �6   .+�5   
   �� $��0��6   $	��Top=�5      $��16(�5      ���	#FFD9FFA2$
��Bold"�5      $��Arial Black)�5      ��5      �� *Notification.Title+5      �� 5    
    �� Orientation$ Vertical=�6   $��1��6,   .+�5!      �� $S��MДата с которой будут обновлятся документы��6   $��18(�6I   (  "System.Windows.Controls.DatePicker  5"      $��18(�5%         SelectedDate 5"      �� *Notification.dateFrom+ �� Mode$ TwoWay=�	   DisplayDate	 5#      �� *Notification.dateFrom+$ OneWay=�
   Text
 5$      �� *Notification.dateFrom+ �� StringFormat$ {0:dd.MM.yyyy}��$ OneWay=�5%      �� 5&      $U��OДата до которой будут обновлятся документы��6   $��18(�6J     5'      $��18(�5*       5'      �� *Notification.dateTo+$ TwoWay=�	 5(      �� *Notification.dateTo+$ OneWay=�
 5)      �� *Notification.dateTo+$ {0:dd.MM.yyyy}��$ OneWay=�5*      5+      �� 5-   
   ���	#FFD9FFA2$��18(�5/      $��2��50      ��51      �� *	OkCommand+.��52      Обновить6   �� 53   
   $��0 0 0 2q�54         CancelButton#��[  55      $��18(�56      $��0��57      $��Right=�58      ��59      �� *CancelCommand+.��5:      ��   MedoClientCancelRedIcon#��[ 6   5;      5<      5=      