   M S B A M L   `   `   `  ���� H  DTabViewModule, Version=1.5.0.0, Culture=neutral, PublicKeyToken=null]3clr-namespace:Medo.Modules.TabViewModule.ViewModels%Medo.Modules.TabViewModule.ViewModels  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=1.3.0.150, Culture=neutral, PublicKeyToken=nullc _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8  �� MergedDictionaries �� Source   ImageCellTemplate �Name � ColumnDefinitions � RowDefinitions � ToolTip �� RelativeSource �� Mode �� AncestorType	 �� Path
 �� ElementName  MedoClientRefreshIcon  MedoClientChangeIcon �� 
TargetType �� Property  MedoClientVisibilityIIcon �� Value �� Triggers �� Binding �� Value  MedoClientVisibilityVIcon �� BasedOn  MedoClientDublicationIcon �� StringFormat �� UpdateSourceTrigger �� Orientation  BytesToBitmapImageConverter �� 	Converter K� ZIndex-   'MahApps.Metro.Controls.MetroProgressBar  PublicationComplieteColor � Maximum � Value � LayoutTransform �� Angle ^� ToolTip  DefferedOpenCellTemplate ��Name �� TextAlignment �� Triggers �� 
TargetName 	 OrganNameCellTemplate 
 MultiValidationTemplate  =� ErrorTemplate  ActTypeCellTemplate  NumberCellTemplate  SignDateCellTemplate  NameCellTemplate! ��Name  Document_UpdateIcon  MedoClientClearIcon  PagesCellTemplate  DeliveryTimeCellTemplate  ComplexNameCellTemplate  EoNumberCellTemplate  PublDateCellTemplate  DaysLeftCellTemplate" � DataContext# �� NotifyOnSourceUpdated  DoublesTypeCellTemplate�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     8metro,http://metro.mahapps.com/winfx/xaml/controls  Bi:http://schemas.microsoft.com/expression/2010/interactivity  :vm3clr-namespace:Medo.Modules.TabViewModule.ViewModels  $prismhttp://prismlibrary.com/    �� $U Opack://application:,,,/Medo.ImageResources;component/PNGResourceDictionary.xaml?��� $T Npack://application:,,,/TabViewModule;component/Styles/MainListView/Colors.xaml?��� $F @pack://application:,,,/Medo.Core;component/Resources/Button.xaml?��� $D >pack://application:,,,/TabViewModule;component/Converters.xaml?��� $@ :pack://application:,,,/Medo.Core;component/Converters.xaml?��� $U Opack://application:,,,/Medo.Core;component/ErrorsValidation/ErrorsTemplate.xaml?�%  &	        7  7  7  7  7��7��7  7  7  &	 j
    &		 5    7 
 &	 �    &	 �    &	 w    &	 ,    7  7  &	     &	 R    &	 �    &	 �    &	 �    &	 �    &	     �� .R�� $
 grid�� �� $��30 ��� $��90 ��� $��15 � �� $��15 ��� $	��80* �.+�� $
��Left=�$��0��$��2��.+��� $	��Top=�$L FОбновление документа напрямую из базы��$
��Left=�����  �� $ FindAncestor=�" ��$6	 0DataContext.UpdateCurrentDocumentFromBaseCommand����� $

 grid��$	 DataContext.HeaderGuid�.���� 8��  $��25���� $��Bottom=�$��0,0,0,50q�$� �Заменить документ (Документ копируется на флеш-носитель и в директорию НТЦ Система)������  �� $ FindAncestor=�" ��$'	 !DataContext.ChangeDocumentCommand����� $

 grid��$	 DataContext�.���� 8�� $��25���� $��Bottom=�$��0,0,0,25q�����  �� $ FindAncestor=�" ��$'	 !DataContext.DeleteDocumentCommand����� $

 grid��$	 DataContext�.���� $��20������ " ��.���  � ��8   ��  �� *IsInvisible+ true.a���  � ��8  ���� " ��8  .���  �@�ToolTip$� �Удалить документ из списка (документ можно будет просмотреть в списке удаленных)�� ��  �� *IsInvisible+ true.a���  �@�ToolTip$� �Восстановить документ из удаленных (Документ будет виден в общем списке)���� $��Bottom=�$@ :Простмотр дубликатов документа������  �� $ FindAncestor=�" ��$$	 DataContext.ViewDoublesCommand����� $

 grid��$	 DataContext����� " ��8   ��  �� *
IsDouble+	 false.a���  � y�$ 	Collapsed=�.���� 8�� $��25���� $��0��$��Center=�$��1��
��� � �$Q KДата обновления документа с сервиса МЭДО������ *
UpdateTime+$ {0:HH:mm:ss}��$ PropertyChanged=��� $ Vertical=�$��1��$��1��.+��� ����  �� $ FindAncestor=�" ��$*	 $DataContext.DocumentSelectionCommand����� $

 grid��$	 DataContext.HeaderGuid�.���� $��Center=�$��89��$��Center=����� *
SourceGuid+8  �� $��2��$��1��$��Center=�$	��Top=�$��14(�$ 2������ *daysLeft.DaysLeftCount+���� " �� ��  �� *daysLeft.DaysLeftCount+ 0.a���  � y�$ 	Collapsed=�   $��15��$��89��8�� ���	#59FF0000$��2��$��1�� �� *daysLeft.Total+ �� *daysLeft.DaysLeftDouble+ �� $	 270Q� �� ���� *daysLeft.DaysLeftString+���� "    ��  �� *daysLeft.Visibility+	 false.a���  � y�$ 	Collapsed=��� .R��� $ datetext��$��Center=�$ Center=����� *DefferedOpen+$ {0:dd.MM.yyyy}�� �� $
 true�� �� *PublDatePortal.HasValue+.a��� $ datetext�� � y�$ Visible=��� .R��� $
��Wrap=�$��Center=�$ Justify=�8    ���� *	OrganName+�� .R��� $
��Wrap=�$��Center=�$ Justify=����� *	ActType+�� .R��� $ Vertical=�$��Center=�$��Center=�.+��� $	 pak������ *DocumentNumber+$ ПАК: {0}���� $	 izd��$��0,3,0,0q����� *ChangedNumber+$ ИЗД: {0}���� $ mj��$
��Left=�$��0,3,0,0q����� *
MJNumber+$ 	МЮ: {0}�� ��  �� *
MJNumber+ Z� .a��� $ mj�� � y�$ Hidden=���  �� *ChangedNumber+ Z� .a��� $	 izd�� � y�$ Hidden=��� .R��� $ Vertical=�$��Center=�$��Center=�.+��� ���� *
SignDate+$ {0:dd.MM.yyyy}���� $ datetext��$��0,20,0,0q����� *MJDate+$ {0:dd.MM.yyyy}���� .R�� $ txtgrid��$	��355�� �� $	��10* ��� $	��80* ��� $	��10* �.+��� $
��Wrap=�$��1��$��Stretch=�$��60������ *DocumentText+�� $! comment��$ Vertical=�$��2��.+��� ���	#FF3A55FF$��0,4,0,0q��� $
��Wrap=�$��Stretch=�$ Justify=����	#FF9DDBFF���� *
Comments+���� " �� ��  �� *
Comments+ Z� .a���  � y�$ Hidden=�� $ buttons��$��0��$��40��.+��� $7 1Восстановить наименование��$��25��$��25��$��Right=�$��0,0,30,0q�����  �� $ FindAncestor=�" ��$&	  DataContext.ReturnActNameCommand����� $	 DataContext� �� " ��.���� 8��  $��25��$
��Left=��� $/ )Очистить наименование��$��25��$��25��$��Right=�����  �� $ FindAncestor=�" ��$&	  DataContext.ChangeActNameCommand���Z� .���� 8��  �� $ false�� �� $	 
IsSelected� �� " ��.a��� $ buttons�� � y�$ Hidden=��� .R��� $��Center=�$ Center=����� *
PagesCount+�� .R��� $ Center=�$��Center=�$��Center=����� *DeliveryTime+$ {0:dd.MM.yyyy - HH:mm}���� .R��� $
��Wrap=�$ Justify=����� *ComplexName+�� .R��� $ 
Horizontal=�$��Center=�.+��� $ Center=�$��Center=�$��5,0,0,0q����� *
EoNumber+ �� .+��� ���� *NotificationSendDate+$; 5Отчет отправлен: {0:dd.MM.yyyy - HH:mm}���� .R��� $ datetext��$ Center=�$��Center=�$��Center=����� *PublDatePortal+$ {0:dd.MM.yyyy - HH:mm}�� �� $
 true�� �� *PublDatePortal.HasValue+.a��� $ datetext�� � y�$ Visible=��� .R�� $
 grid��$��Center=�.+��� $ Vertical=�" �� $	 DaysLeftClass�# . y��� *
Visibility+���� " �� ��  �� *
Visibility+	 false.a���  � y�$ 	Collapsed=�.+��� ���� *DaysLeftString+   $��10��$	��100�����	#FF5FEC1D���	#59FF0000 �� *Total+ �� *DaysLeftDouble+�� .R��� $��Center=����� *DoubleIdentification+