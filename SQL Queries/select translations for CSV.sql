use [DeadSeaCatalogueDAL.ProductContext]

select t.title, t.[desc], p.artikul, 
70 * CONVERT(float,replace(p.price, '$','')) as price, 
--'http://www.israel-catalog.com/sites/default/files/products/images/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru/product_gallery/' + p.imageFileName as imageFileName
--'/product_gallery/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru.xsph.ru/wp-content/uploads/2016/05/' + p.imageFileName as imageFileName
p.imageFileName as imageFileName
from Products p
inner join Translations t on t.titleEng = p.title
order by t.title