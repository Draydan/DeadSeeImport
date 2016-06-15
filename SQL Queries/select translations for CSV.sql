use [DeadSeaCatalogueDAL.ProductContext]

select --p.title, t.titleEng, 
t.title, dbo.replacenewline(t.[desc], '<br>'), p.artikul, 
65 * CONVERT(float,replace(p.price, '$','')) as price, 
--'http://www.israel-catalog.com/sites/default/files/products/images/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru/product_gallery/' + p.imageFileName as imageFileName
--'/product_gallery/' + p.imageFileName as imageFileName
--'http://izrael-cosmetics.ru.xsph.ru/wp-content/uploads/2016/05/' + p.imageFileName as imageFileName
p.imageFileName as imageFileName
from Products p
inner join Translations t on t.titleEng = p.title
order by t.title

select * from
[DeadSeaCatalogueDAL.ProductContext].dbo.Translations t
where titleEng = '' or isnull(titleEng, '')=''
and not exists 
(select * from Translations t2 where t.title = t2.title and t2.titleEng is not null)
order by t.title


select dbo.replacenewline('sdf', 'a')