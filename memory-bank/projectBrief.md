# Project Brief: Georgia Tech Library Marketplace

## Core Project Definition
Georgia Tech Library will transform its traditional bookstore into a web-based marketplace empowering students with affordable and accessible textbooks worldwide, while creating a platform where graduates can monetize their surplus materials.

## Mission
Empowering education through affordable and accessible textbooks, we foster a seamless online marketplace for students to buy and sell, cultivating a global community of learners.

## Vision
To be the unrivaled global webshop for literature, inspiring and supporting students across universities with a seamless and affordable marketplace.

## 5-Year Business Objectives
- Achieve profitability within the next year followed by 20% annual revenue growth
- Expand across the US and EU within 2 years, global expansion within 5 years
- Continuously identify and implement new features and revenue streams
- Become the most reliable source of student literature
- Create a sustainable organization that supports creativity and innovation

## Core Requirements
The marketplace enables students to buy both new textbooks (from GTL's inventory) and used textbooks (sold by graduates) through a single platform, with the business taking a small percentage on each transaction.

## Technical Scope
Build a scalable microservices architecture capable of:
- Handling 1000 requests per minute baseline
- Search response under 1 second regardless of catalog size
- Managing traffic spikes during semester starts
- Supporting multi-vendor orders (single purchase from multiple sellers)
- Scaling globally across continents while maintaining high availability

## Key Business Challenges
- Complex order fulfillment when customers buy from multiple sellers simultaneously
- Maintaining stock accuracy across distributed services
- Ensuring search results reflect real-time availability
- Coordinating shipping notifications to individual sellers
- Managing both bookstore warehouse operations and marketplace listings

## Key Stakeholders
- **Board of Directors**: Drive profitability focus, global expansion vision, potential organizational structure changes
- **Development Team (10 people)**: Functional structure, build/maintain microservices with messaging
- **Students**: Access affordable books through seamless search and purchase experience
- **Graduates**: Simple platform to list and sell their used textbooks
- **Academic Institutions**: Reliable literature marketplace for their students

This brief establishes the foundation for the GTL marketplace transformation, requiring sophisticated distributed systems to manage marketplace complexity while delivering textbook affordability at scale.
