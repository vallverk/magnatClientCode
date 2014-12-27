#!/bin/bash

( 
	echo "cd /var/www/html 
   rm ./*.unity3d
   rm ./*.png
   rm ./*.html
   put /Users/mm1/Documents/Projects/MagnatGame/frontend/Magnat/Builds/WebPlayer/*.unity3d
   put /Users/mm1/Documents/Projects/MagnatGame/frontend/Magnat/Builds/WebPlayer/*.png
   put /Users/mm1/Documents/Projects/MagnatGame/frontend/Magnat/Builds/WebPlayer/*.html
   bye
	"
) | sftp rad@magnatgame.com 