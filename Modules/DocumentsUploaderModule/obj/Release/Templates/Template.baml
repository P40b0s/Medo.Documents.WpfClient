   M S B A M L   `   `   `  ���� R  NDocumentsUploaderModule, Version=1.4.0.0, Culture=neutral, PublicKeyToken=nullq=clr-namespace:Medo.Modules.DocumentsUploaderModule.Converters/Medo.Modules.DocumentsUploaderModule.Converters  \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=1.3.0.150, Culture=neutral, PublicKeyToken=nullP LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8  �� MergedDictionaries �� SourceJ   DMedo.Modules.DocumentsUploaderModule.Converters.ByteToImageConverter   BytesToImageConverter  ListBoxItemBackground �� 
TargetType �� Property �� Value �� Triggers �� Binding �� Value :� RoutedEvent	 �� TargetProperty
 g� Duration W� From	 W� To  ListItem �� DataType �Name � ColumnDefinitions � RowDefinitions �� Orientation ��Name �� 	Converter ��Name �� Triggers �� 
TargetName  PdfErrorIcon  XmlErrorIcon�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     ;Controls,http://metro.mahapps.com/winfx/xaml/controls  @i:http://schemas.microsoft.com/expression/2010/interactivity  L
converters=clr-namespace:Medo.Modules.DocumentsUploaderModule.Converters  $prismhttp://prismlibrary.com/    �� $U Opack://application:,,,/Medo.ImageResources;component/PNGResourceDictionary.xaml?�%�  &	        &	      &	 R    7   7  7     �� " ��.���  � �� �	#FFDCFF92 ��  �� *IsCopyComplete+	 false.a���  � �� �	#FFFF8C8C:� $ FrameworkElement.Loaded��.[��� .���� .��W� $	 Opacity�$
 00:00:03D�$ 0X�$ 1X�:� $ FrameworkElement.Unloaded��.[��� .���� .��W� $	 Opacity�$
 00:00:02D�$ 1X�$ 0X��� " ��.R��� ���	#CCFFFFFF$��0,0,0,3q�._�� $
 grid�� �� $��90 ��� $	��700 � �� $��15 ��� $��* �.+��� $ 
Horizontal=�$��0��$��0��.+��� $ xmlImage��$��15��$y�	Collapsed=��� $ pdfImage��$��15��$y�	Collapsed=��� $��Center=�$��89��$��Center=�$��0��$��1������ *
SourceGuid+8   �� $ Vertical=�$��1��$��1��.+��� $
��Wrap=�
���   �$��Calibri)�$��16(����� *	OrganName+�� $
��Wrap=�
���   �$��Calibri)�$��16(����� *	ActType+�� $ 	pakNumber��$
��Wrap=�
���   �$��Calibri)�$��16(����� *DocumentNumber+�� $ 	izdNumber��$
��Wrap=�
���   �$��Calibri)�$��16(����� *ChangedNumber+�� $
��Wrap=�
���   �$��Calibri)�$��16(����� *CopyMessage+ ��  �� *DocumentNumber+ Z� .a��� $ 	pakNumber�� � y�$ 	Collapsed=���  �� *ChangedNumber+ Z� .a��� $ 	izdNumber�� � y�$ 	Collapsed=��� $ false�� �� *	PdfIsCopy+.a��� $ pdfImage��8   � ���� $ pdfImage�� �@�ToolTip$$ PDF файл не найден���� $ pdfImage�� � y�$ Visible=��� $ false�� �� *	XmlIsCopy+.a��� $ xmlImage��8   � ���� $ xmlImage�� �@�ToolTip$8 2Ошибка создания XML карточки���� $ xmlImage�� � y�$ Visible=�