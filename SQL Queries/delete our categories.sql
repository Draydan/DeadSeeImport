delete from LinkProductWithCategories
where category_ID in
(select id from Categories
where isOurCategory = 1)

delete from Categories
where isOurCategory = 1