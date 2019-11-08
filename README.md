# AME
An assessment tool which generates and reads PDF files, storing student response data in a compactSQL database. 


## Why?
I wanted automate marking. At the time, students were only using phones, not tablets. I wanted to send them an electronic 'worksheet' and read the data off it so I chose the PDF specification and learned how to create and read PDF's.

## Which bits are you most proud of?
* The two layer system where entities which plug in to Entity Framework are encapsulated in the bottom data access layer. The middle layer is the business logic and the top layer is the view.
* Implementing MVVM
* [Response.cs](https://github.com/kierenholt/AME/blob/master/AME_base/Entities/Response.cs) - Composite Keys massively sped up join queries and also reduced database size
* [Book.xaml](https://github.com/kierenholt/AME/blob/master/AME_addin/markbook/Book.xaml) - dynamically generated XAML
* [CellViewModel.cs](https://github.com/kierenholt/AME/blob/master/AME_base/ViewModel/grid/CellViewModel.cs) - the markbook was  a data table viewmodel which contained lots of subviewmodels which held a generic entity class 

## What happened to it?
Things were moving quickly onto the cloud. Each student was to receive their own Chromebook so I felt that a cloud based solution might work better, with less file storage / sending overhead. So I wrote Teachometer which borrowed some concepts but the code base was entirely new.
