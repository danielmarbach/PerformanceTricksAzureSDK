# Performance tricks I learned from contributing to the Azure .NET SDK

As a practical learner, I've found that performance optimizations have been my biggest challenge and the place I've learned the tricks that are the most helpful. These lessons have come by trial and error. As it turns out, the Azure .NET SDK was a perfect “playground” for learning new tricks because it's maintained by people who care and give feedback. 

Over the past few years, I've contributed over fifty pull requests to the Azure .NET SDK and some of them got accepted. In this session, I'll walk you through the performance improvements I learned from my experiments. Some “superpowers” you will learn are spotting closure allocations and opportunities for memory pooling and, best of all, how to improve them. 
