   M S B A M L   `   `   `  ����I  ETrayInfoModule, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:Medo.Modules.TrayInfoModule.Converters&Medo.Modules.TrayInfoModule.Converters  5      ;    5Medo.Modules.TrayInfoModule.Views.NotificationControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      I
convertres4clr-namespace:Medo.Modules.TrayInfoModule.Converters    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        � 	Resources  5      A  ;Medo.Modules.TrayInfoModule.Converters.ByteToImageConverter   btoimgconverter  NotificationStyle �� 
TargetType �� Property �� Value �� RelativeSource �� AncestorType �� 
TargetType ��Name �� Orientation	 J� Opacity
 �� ElementName �� Path �� 	Converter �� TextAlignment �� StringFormat �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value :� RoutedEvent �� TargetProperty g� Duration W� From	 W� To��%F
  &	        &	      7     5   
   6*   �� 5   
   " ��6*   .�5      ��  � ��
 �   �6+   �� 5	       � �� 6;   �� *HorizontalContentAlignment+ �� " ���� 5
       � �� 69   �� *VerticalContentAlignment+ �� " ���� 5       � �� 5      �� 5      " ��6&   .R�5      �� $ 
mainBorder��6!   
������ $��2q�6P   ._�5      �� $ 
Horizontal=�6)   $	��450��6B   
�������$
	 0.85Q�6a   .+�5   "   �� $��0,0,1,0q�6j   ��6)   �� $
 
mainBorder��$ BorderBrush�._�5   &   �� $��60��6v   ��6,   �� *
SourceGuid+8   6v   5   #   �� 5   "   $��0,0,1,0q�6j   ��6)   �� $
 
mainBorder��$ BorderBrush�._�5   &   �� $
��Wrap=�60   $	��150��6D   $ Center=�6k   $��Center=�6�   $��Calibri)�6�   $
��Bold"�6�   $��14(�6�   ��6P   �� *	OrganName+6�   5   #   �� 5   "   $��0,0,1,0q�6j   ��6)   �� $
 
mainBorder��$ BorderBrush�._�5   &   �� $
��Wrap=�60   $	��100��6D   $ Center=�6i   $��Center=�6�   $��Calibri)�6�   $
��Bold"�6�   $��14(�6�   ��6P   �� *	ActType+6�   5   #   �� 5   "   $��0,0,1,0q�6j   ��6)   �� $
 
mainBorder��$ BorderBrush�._�5   &   �� $
��Wrap=�60   $��70��6D   $ Center=�6j   $��Center=�6�   $��Calibri)�6�   $
��Bold"�6�   $��14(�6�   ��6O   �� *	ActNumber+6�   5   #   �� 5   "   $
��Wrap=�6,   $��70��6@   $ Center=�6�   $��Center=�6�   $��Calibri)�6�   $
��Bold"�6�   $��14(�6�   ��6K   �� *DeliveryTime+$ {0:dd.MM.yyyy HH:mm:ss}��6�   5      5        5!      P� 5"       �@��
IsSelected . . �5#   "   �� $ 
mainBorder��6?   # � ��6W    � ���� 5$   "    � ��
 �   �6?   5%      P� 5&       �@J�IsMouseOver . . �5'   "   �� $ 
mainBorder��6)    � ��
 � � �6W   5(      ]� 5)       5*   "   �� 5+   &    �@��
IsSelected . 6F   �� 5,   &    �@��IsSelectionActive .  6V   ./�5.   "   �� $ 
mainBorder��6?   # � ��6W    � ���� 5/   "    � ��
 �   �6?   50      P� 51       � {� .  . �52   "   �� # � ��6?    � ��53      :� 55      $ FrameworkElement.Loaded��6+   .[�56   "   �� .��57   &   �� .��58   *   W� $ Opacity�6:   $ 00:00:02D�59   *   $ 0X�5:   *   $ 1X�5;   *   5<   '   5=   #   5>      :� 5@      $ FrameworkElement.Unloaded��6+   .[�5A   "   �� .��5B   &   �� .��5C   *   W� $ Opacity�6:   $ 00:00:02D�5D   *   $ 1X�5E   *   $ 0X�5F   *   5G   '   5H   #   5I      5K      5M      5N      5P      .��5Q      �� -   5         ��Name$ itemsListBox��5Q       �� SelectionMode$ Single=�6$   
������ 
������ #��[ 6o   ��6�   �� *NotificationsCollection+5R      