   M S B A M L   `   `   `  ����L  HReportsListModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Medo.Modules.ReportListModule.Converters(Medo.Modules.ReportListModule.Converters  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8J   DMedo.Modules.ReportListModule.Converters.BytesToBitmapImageConverter   bytesToImageConverter  ButtonCellTemplate  �� Orientation �� RelativeSource �� AncestorType �� Path �� 	Converter  OrganNameCellTemplate �� TextAlignment  ActTypeCellTemplate  NumberCellTemplate  SignDateCellTemplate �� StringFormat  ComplexNameCellTemplate  EoNumberCellTemplate  PublicationDateCellTemplate "	 AutoSendNotifyDateCellTemplate $
  ManualSendNotifyDateCellTemplate �� ItemStringFormat�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      E
converters6clr-namespace:Medo.Modules.ReportListModule.Converters  5      Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      $prismhttp://prismlibrary.com/  5      %  &	        &	      7   &	     &	 �    &	 +    &	 �    &	 �    &	 5    &	 �    &		 A    &	
 -       5	      6-   �� 5      .R�5   
   �� $  Vertical=�6   .+�5      �� ��6   ��  �� " ��$% DataContext.SelectReportCommand���6�   �� *NotificationGuid+.��5      �� $��80��6h   ��6   �� *
SourceGuid+8   6h   5      5      5      �� 5      .R�5   
   �� $
��Wrap=�64   $ Center=�6I   $��Center=�6`   ��6   �� $ 	OrganName�6`   5      �� 5      .R�5   
   �� $
��Wrap=�62   $��Center=�6F   $ Center=�6d   $��Center=�6{   ��6   �� $ ActType�6{   5      �� 5      .R�5   
   �� $  Vertical=�6   $��Center=�6,   $��Center=�6G   .+�5      �� $ Center=�66   $��Center=�6M   ��6   �� $ Number�6M   5      5      �� 5!      .R�5"   
   �� $  Vertical=�6   $��Center=�6,   $��Center=�6G   .+�5#      �� $ Center=�6W   $��Center=�6n   ��6   �� $ SignDate�$ {0:dd.MM.yyyy}��6n   5$      5%      �� 5'      .R�5(   
   �� .+�5)      �� $
��Wrap=�6:   ��6   �� $ ComplexName�6:   5*      5+      �� 5-      .R�5.   
   �� $  Vertical=�6   $��Center=�6,   .+�5/      �� $ Center=�67   $��Center=�6N   $��Center=�6k   $��5,0,0,0q�6�   ��6   �� $ EoNumber�6�   �� 50      �� 51      $ Center=�65   $��Center=�6L   $
��Wrap=�6i   $��Center=�6}   $��5,0,0,0q�6�   ��6   �� $ Status�6�   52      53      �� 55      .R�56   
   �� $ Center=�6c   $��Center=�6z   $��Center=�6�   ��6   �� $ PublicationDate�$ {0:dd.MM.yyyy - HH:mm}��6�   57      �� 59      .R�5:   
   �� $  
Horizontal=�6   $��Center=�6.   $��Center=�6K   .+�5;      �� $��Center=�6k   $ Center=�6�   ��6   �� $ NotificationSendDate�$  {0:dd.MM.yyyy - HH:mm}��6�   5<      5=      �� 5?      .R�5@   
   �� $  
Horizontal=�6   $��Center=�6.   $��Center=�6K   $	��140��6f   .+�5A      �� 
������ $ {0:dd.MM.yyyy - HH:mm}��6[   ��6/   �� $ ManualSendTime�6[   5B      5C      5E      