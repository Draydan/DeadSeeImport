   M S B A M L   `   `   `  ����G  CDeadSeaTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:DeadSeaToolsDeadSeaTools  5   	       DeadSeaTools.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:DeadSeaTools     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5	      � .+�5
   
   �� -   5         ��Name$ 
tabControl��5
      $��	10,16,0,0q�6)   .@�5      �� -   5        $��Import URLs��5       J� RenderTransformOrigin$ 3.227,0.863&�6V   .��5      � ���	#FFE5E5E5.+�5      �� -   5         ��Name$ cbPath��5      $
��Left=�6/   $��0,10,0,0q�6J   $	��Top=�6\   $	��300��6t    �� 
IsEditable . 6�   �� 5      -   5         ��Name$ 	btnBrowse��5      $��Browse��60   $
��Left=�6A   $��
312,10,0,0q�6\   $	��Top=�6p   $��75��6�   6�   �� 5      -   5        $ 	btnImport��5      $��Import��60   $
��Left=�6A   $��
402,10,0,0q�6\   $	��Top=�6p   $��75��6�   6�   �� 5      -   5         ��Name$ 	lbLogUrls��5      $��Stretch=�61   $��0,37,4,6q�6O   $��Stretch=�6a   5      5      �� 5      -   5        $��Export Ya.Market��5      .��5      � ���	#FFE5E5E5.+�5      �� -   5        $ bExport��5      $��	Export4YM��6.   $
��Left=�6B   $��
402,10,0,0q�6]   $	��Top=�6q   $��75��6�   6�   �� 5      -	   5        $ cbExportYMFile��5      $
��Left=�67   $��10,9,0,0q�6R   $	��Top=�6d   $	��284��6|    . 6�   �� 5      -
   5        $ bBrowseExportFile��5      $��Browse��68   $
��Left=�6I   $��
309,10,0,0q�6d   $	��Top=�6x   $��75��6�   6�   5      5      �� 5      -   5        $��Get roboted IK��5      $��93��6.   .��5      � ���	#FFE5E5E5.+�5      �� -   5        $ cbRobotPath��5      $
��Left=�64   $��9,10,0,0q�6O   $	��Top=�6a   $	��291��6y    . 6�   �� 5      -   5        $ bBrowseIKRobot��5      $��Browse��65   $
��Left=�6F   $��
313,10,0,0q�6a   $	��Top=�6u   $��75��6�   6�   �� 5      -   5        $ 
bExport2DB��5      $��	Import2DB��61   $
��Left=�6E   $��
402,10,0,0q�6`   $	��Top=�6t   $��75��6�   6�   �� 5      -   5        $ 
lbLogRobot��5      $��	10,46,0,0q�62   5       5!      &   System.Windows.Controls.DataGrid  5"      -   5          Name$ dataGrid��5"      $	��100��6)   $	��100��66   �� 5#      -   5        $��Import Kontrakt��5#      $
��Left=�6/   $��20��6J   $	��Top=�6V   $��98��6n   $��-1,0,0,0q�6y   .��5$      � ���	#FFE5E5E5.+�5%      �� -   5        $ cbKontraktPath��5%      $
��Left=�67   $��	10,17,0,0q�6R   $	��Top=�6e   $	��297��6}    . 6�   �� 5&      -   5        $ bBrowseKontrakt��5&      $��Browse��66   $
��Left=�6G   $��
321,17,0,0q�6b   $	��Top=�6v   $��53��6�   6�   �� 5'      -   5        $ bImportKontrakt��5'      $��Import Kontrakt��66   $
��Left=�6P   $��
388,17,0,0q�6k   $	��Top=�6   $��99��6�   6�   �� 5(      -   5         ��Name$ 
tbKontrakt��5(      	 �� TextWrapping$
	 Wrap=�62   $��TextBox��6F   $��0,55,0,0q�6U   5)      5*      5+      5.      5/      