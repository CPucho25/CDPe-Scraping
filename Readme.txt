
El sistema de CDP-Scraping (Comprobante de Pago), consta de 3 procesos:
	1) Divide archivos txt
	2) Scraping a la web de Sunat-CDP(consulta y descarga archivos txt)
	3) Une archivos txt descargados de la web en uno solo

El sistema al ejecutar creara 4 carpetas dentro del proyecto.

//Codigo maestro
CDPe-Scraping\CDPe-Scraping\bin\Debug\
ó
//Ejecutable
CDPe-Scraping\CDPe-Scraping\bin\Release\

Descripcion de Carpetas:

1-CDPs_Input_Split (Aqui se coloca el archivo FUENTE con total de registros a consultar)
2-CDPs_Ouput_Split (El sistema automaticamente dividira el archivo fuente de 100 en 100 lineas y guardará en esta carpeta)
3-CDPs-Downloads (Automaticamente los archivos descargados de la web se almacenaran en esta carpeta)
4-CDPs-Ouput_Join (Todos los archivos descargados en la capeta 3) se uniran en uno y se guardará en esta carpeta)

Nota: Si la web detecta que los documentos consultados presentan algun tipo de error, en la carpeta 4) se generara un documento txt con la descripcion del error.
