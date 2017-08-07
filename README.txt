{\rtf1\ansi\ansicpg1252\cocoartf1348\cocoasubrtf170
{\fonttbl\f0\fswiss\fcharset0 Helvetica;}
{\colortbl;\red255\green255\blue255;}
\margl1440\margr1440\vieww10800\viewh8400\viewkind0
\pard\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\pardirnatural

\f0\fs24 \cf0 Cylinder\
	The cylinder is generated based on half-axis, radius, and cap resolution that are exposed as public properties.  First the cylinder is created based on a half-axis with the unit up vector given the radius and cap resolution.  Then it is rotated to the half-axis and scaled appropriately.\
\
Point in Mesh Checker\
	You can check what point lies in the mesh by creating an instance and passing in a world point through the method call ContainsWorldPoint.  A debug draw ray is drawn in red to show a better visual way of seeing if the world point does lie on the cylinder or inside it where the end of the ray is the world point and the beginning of it is the center of the cylinder.\
\
Fur shader\
	You can change the wind speed\'92s x, y, and z in the fur material to add a wind force animation on the fur.  You can also increase the fur\'92s length in the fur material by small increments to make the fur look thinner.\
}