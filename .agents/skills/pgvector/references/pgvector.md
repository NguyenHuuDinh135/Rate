# Pgvector - Pgvector

**Pages:** 50

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-python/blob/master/examples/citus/example.py

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/stargazers

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/LICENSE

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-algol

**Contents:**
- pgvector-algol
- Getting Started
- Algol 68 Genie
- Contributing

pgvector examples for Algol

Supports Algol 68 Genie

Follow the instructions for your database library:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

**Examples:**

Example 1 (unknown):
```unknown
pq exec (db, "CREATE EXTENSION IF NOT EXISTS vector");
```

Example 2 (unknown):
```unknown
pq exec (db, "CREATE TABLE items (id bigserial PRIMARY KEY, embedding vector(3))");
```

Example 3 (unknown):
```unknown
pq exec (db, "INSERT INTO items (embedding) VALUES ('[1,2,3]'), ('[4,5,6]')");
```

Example 4 (unknown):
```unknown
pq exec (db, "SELECT id FROM items ORDER BY embedding <-> '[3,1,2]' LIMIT 5");
FOR i TO pq ntuples (db)
DO pq getvalue (db, i, 1);
  print((res, newline))
OD;
```

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-dart

**Contents:**
- pgvector-dart
- Getting Started
- postgres
- Reference
  - Vectors
  - Half Vectors
  - Binary Vectors
  - Sparse Vectors
- History
- Contributing

pgvector support for Dart

Supports the postgres package

And follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a vector from a list

Create a half vector from a list

Create a binary vector from a list

Create a sparse vector from a list

Or a map of non-zero elements

Note: Indices start at 0

Get the number of dimensions

Get the indices of non-zero elements

Get the values of non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/Makefile.win

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-cpp

**Contents:**
- pgvector-cpp
- Installation
- Getting Started
- libpqxx
- Reference
  - Vectors
  - Half Vectors
  - Sparse Vectors
- History
- Contributing

pgvector support for C++

Add the headers to your project (supports C++17 and greater).

There is also support for CMake and FetchContent:

Follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Use std::optional<pgvector::Vector> if the value could be NULL

Create a vector from a std::vector<float>

Convert to a std::vector<float>

Create a half vector from a std::vector<float>

Convert to a std::vector<float>

Create a sparse vector from a std::vector<float>

Or a map of non-zero elements

Get the number of dimensions

Get the indices of non-zero elements

Get the values of non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-d

**Contents:**
- pgvector-d
- Getting Started
- dpq2
- Contributing

pgvector examples for D

Follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

Specify the path to libpq if needed:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-lisp

**Contents:**
- pgvector-lisp
- Getting Started
- Postmodern
- CL-DBI
- Contributing

pgvector examples for Common Lisp

Supports Postmodern and CL-DBI

Follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/tree/master/src

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-python/blob/master/examples/hybrid_search/rrf.py

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-haskell

**Contents:**
- pgvector-haskell
- Getting Started
- postgresql-simple
- History
- Contributing

pgvector support for Haskell

Supports postgresql-simple

Add this line to your application’s .cabal file under build-depends:

And follow the instructions for your database library:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

**Examples:**

Example 1 (unknown):
```unknown
pgvector >= 0.1 && < 0.2
```

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-c

**Contents:**
- pgvector-c
- Getting Started
- libpq
- Contributing

pgvector examples for C

Follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/pulse

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/README.md

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-elixir

**Contents:**
- pgvector-elixir
- Installation
- Ecto
- Postgrex
- Reference
  - Vectors
  - Half Vectors
  - Sparse Vectors
- Upgrading
  - 0.3.0

pgvector support for Elixir

Supports Ecto and Postgrex

Add this line to your application’s mix.exs under deps:

And follow the instructions for your database library:

Or check out some examples:

Create lib/postgrex_types.ex with:

And add to config/config.exs:

You can now use the vector type in future migrations

Also supports :halfvec, :bit, and :sparsevec

Also supports Pgvector.Ecto.HalfVector, Pgvector.Ecto.Bit, and Pgvector.Ecto.SparseVector

Get the nearest neighbors

Also supports max_inner_product, cosine_distance, l1_distance, hamming_distance, and jaccard_distance

Convert a vector to a list or Nx tensor

Add an approximate index in a migration

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Register the extension

And pass it to start_link

Get the nearest neighbors

Convert a vector to a list or Nx tensor

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a vector from a list

Create a half vector from a list

Create a sparse vector from a list

Or a map of non-zero elements

Note: Indices start at 0

Get the number of dimensions

Get the indices of non-zero elements

Get the values of non-zero elements

Lists must be converted to Pgvector structs for Ecto distance functions.

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/tree/master/.github/workflows

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-crystal

**Contents:**
- pgvector-crystal
- Getting Started
- crystal-pg
- Contributing

pgvector examples for Crystal

Follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-dotnet

**Contents:**
- pgvector-dotnet
- Getting Started
- Npgsql (C#)
- Dapper
- Entity Framework Core
- Npgsql.FSharp
- Npgsql (Visual Basic)
- Reference
  - Vectors
  - Half Vectors

pgvector support for .NET (C#, F#, and Visual Basic)

Supports Npgsql, Dapper, Entity Framework Core, and Npgsql.FSharp

Follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Also supports HalfVector and SparseVector

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

The latest version works with .NET 8 and 9. For .NET 6 and 7, use version 0.1.2 and this readme.

Configure the connection

Also supports HalfVector and SparseVector

Get the nearest neighbors

Also supports MaxInnerProduct, CosineDistance, L1Distance, HammingDistance, and JaccardDistance

Get items within a certain distance

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a vector from an array

Create a half vector from an array

Create a sparse vector from an array

Or a dictionary of non-zero elements

Note: Indices start at 0

Get the number of dimensions

Get the indices of non-zero elements

Get the values of non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-java

**Contents:**
- pgvector-java
- Getting Started
- JDBC (Java)
- Spring JDBC
- Hibernate
- R2DBC
- JDBC (Kotlin)
- JDBC (Groovy)
- Groovy SQL
- JDBC (Scala)

pgvector support for Java, Kotlin, Groovy, and Scala

Supports JDBC, Spring JDBC, Groovy SQL, and Slick

For Maven, add to pom.xml under <dependencies>:

For sbt, add to build.sbt:

For other build tools, see this page.

And follow the instructions for your database library:

Or check out some examples:

Import the PGvector class

Register the types with your connection

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Import the PGvector class

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Hibernate 6.4+ has a vector module (use this instead of com.pgvector.pgvector).

For Maven, add to pom.xml under <dependencies>:

Get the nearest neighbors

R2DBC PostgreSQL 1.0.3+ supports the vector type (use this instead of com.pgvector.pgvector).

For Maven, add to pom.xml under <dependencies>:

Import the PGvector class

Register the types with your connection

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Import the PGvector class

Register the types with your connection

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Import the PGvector class

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Import the PGvector class

Register the types with your connection

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Import the PGvector class

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a vector from an array

Create a half vector from an array

Create a binary vector from a byte array

Get the length (number of bits)

Create a sparse vector from an array

Or a map of non-zero elements

Note: Indices start at 0

Get the number of dimensions

Get the indices of non-zero elements

Get the values of non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/pulls

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/META.json

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/CHANGELOG.md

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-node

**Contents:**
- pgvector-node
- Installation
- node-postgres
- Knex.js
- Objection.js
- Kysely
- Sequelize
- pg-promise
- Prisma
- Postgres.js

pgvector support for Node.js, Deno, and Bun (and TypeScript)

Supports node-postgres, Knex.js, Objection.js, Kysely, Sequelize, pg-promise, Prisma, Postgres.js, Slonik, TypeORM, MikroORM, Drizzle ORM, and Bun SQL

And follow the instructions for your database library:

Or check out some examples:

Register the types for a client

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Also supports maxInnerProduct, cosineDistance, l1Distance, hammingDistance, and jaccardDistance

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Also supports maxInnerProduct, cosineDistance, l1Distance, hammingDistance, and jaccardDistance

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Also supports maxInnerProduct, cosineDistance, l1Distance, hammingDistance, and jaccardDistance

Get items within a certain distance

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Also supports maxInnerProduct, cosineDistance, l1Distance, hammingDistance, and jaccardDistance

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Note: prisma migrate dev does not support pgvector indexes

Add the extension to the schema

Add a vector column to the schema

Get the nearest neighbors to a vector

See a full example (and the schema)

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Get the nearest neighbors to a vector

Also supports maxInnerProduct, cosineDistance, l1Distance, hammingDistance, and jaccardDistance

Drizzle ORM 0.31.0+ has built-in support for pgvector 🎉

Also supports halfvec, bit, and sparsevec

Get the nearest neighbors to a vector

Also supports innerProduct, cosineDistance, l1Distance, hammingDistance, and jaccardDistance

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a sparse vector from an array

Or a map of non-zero elements

Note: Indices start at 0

Get the number of dimensions

Get the indices of non-zero elements

Get the values of non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/vector.control

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/security

**Contents:**
- Security Policy

For security issues, contact security@pgvector.com.

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-python/blob/master/examples/loading/example.py

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-ada

**Contents:**
- pgvector-ada
- Getting Started
- GNATcoll
- Contributing

pgvector examples for Ada

Follow the instructions for your database library:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/activity

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/tree/master/test

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector

**Contents:**
- pgvector
- Installation
  - Linux and Mac
  - Windows
- Getting Started
- Storing
- Querying
    - Distances
    - Aggregates
- Indexing

Open-source vector similarity search for Postgres

Store your vectors with the rest of your data. Supports:

Plus ACID compliance, point-in-time recovery, JOINs, and all of the other great features of Postgres

Compile and install the extension (supports Postgres 13+)

See the installation notes if you run into issues

You can also install it with Docker, Homebrew, PGXN, APT, Yum, pkg, APK, or conda-forge, and it comes preinstalled with Postgres.app and many hosted providers. There are also instructions for GitHub Actions.

Ensure C++ support in Visual Studio is installed and run x64 Native Tools Command Prompt for VS [version] as administrator. Then use nmake to build:

See the installation notes if you run into issues

You can also install it with Docker or conda-forge.

Enable the extension (do this once in each database where you want to use it)

Create a vector column with 3 dimensions

Get the nearest neighbors by L2 distance

Also supports inner product (<#>), cosine distance (<=>), and L1 distance (<+>)

Note: <#> returns the negative inner product since Postgres only supports ASC order index scans on operators

Create a new table with a vector column

Or add a vector column to an existing table

Also supports half-precision, binary, and sparse vectors

Or load vectors in bulk using COPY (example)

Get the nearest neighbors to a vector

Supported distance functions are:

Get the nearest neighbors to a row

Get rows within a certain distance

Note: Combine with ORDER BY and LIMIT to use an index

For inner product, multiply by -1 (since <#> returns the negative inner product)

For cosine similarity, use 1 - cosine distance

Average groups of vectors

By default, pgvector performs exact nearest neighbor search, which provides perfect recall.

You can add an index to use approximate nearest neighbor search, which trades some recall for speed. Unlike typical indexes, you will see different results for queries after adding an approximate index.

Supported index types are:

An HNSW index creates a multilayer graph. It has better query performance than IVFFlat (in terms of speed-recall tradeoff), but has slower build times and uses more memory. Also, an index can be created without any data in the table since there isn’t a training step like IVFFlat.

Add an index for each distance function you want to use.

Note: Use halfvec_l2_ops for halfvec and sparsevec_l2_ops for sparsevec (and similar with the other distance functions)

Specify HNSW parameters

A higher value of ef_construction provides better recall at the cost of index build time / insert speed.

Specify the size of the dynamic candidate list for search (40 by default)

A higher value provides better recall at the cost of speed.

Use SET LOCAL inside a transaction to set it for a single query

Indexes build significantly faster when the graph fits into maintenance_work_mem

A notice is shown when the graph no longer fits

Note: Do not set maintenance_work_mem so high that it exhausts the memory on the server

Like other index types, it’s faster to create an index after loading your initial data

You can also speed up index creation by increasing the number of parallel workers (2 by default)

For a large number of workers, you may need to increase max_parallel_workers (8 by default)

The index options also have a significant impact on build time (use the defaults unless seeing low recall)

Check indexing progress

The phases for HNSW are:

An IVFFlat index divides vectors into lists, and then searches a subset of those lists that are closest to the query vector. It has faster build times and uses less memory than HNSW, but has lower query performance (in terms of speed-recall tradeoff).

Three keys to achieving good recall are:

Add an index for each distance function you want to use.

Note: Use halfvec_l2_ops for halfvec (and similar with the other distance functions)

Specify the number of probes (1 by default)

A higher value provides better recall at the cost of speed, and it can be set to the number of lists for exact nearest neighbor search (at which point the planner won’t use the index)

Use SET LOCAL inside a transaction to set it for a single query

Speed up index creation on large tables by increasing the number of parallel workers (2 by default)

For a large number of workers, you may also need to increase max_parallel_workers (8 by default)

Check indexing progress

The phases for IVFFlat are:

Note: % is only populated during the loading tuples phase

There are a few ways to index nearest neighbor queries with a WHERE clause.

A good place to start is creating an index on the filter column. This can provide fast, exact nearest neighbor search in many cases. Postgres has a number of index types for this: B-tree (default), hash, GiST, SP-GiST, GIN, and BRIN.

For multiple columns, consider a multicolumn index.

Exact indexes work well for conditions that match a low percentage of rows. Otherwise, approximate indexes can work better.

With approximate indexes, filtering is applied after the index is scanned. If a condition matches 10% of rows, with HNSW and the default hnsw.ef_search of 40, only 4 rows will match on average. For more rows, increase hnsw.ef_search.

Starting with 0.8.0, you can enable iterative index scans, which will automatically scan more of the index when needed.

If filtering by only a few distinct values, consider partial indexing.

If filtering by many different values, consider partitioning.

With approximate indexes, queries with filtering can return less results since filtering is applied after the index is scanned. Starting with 0.8.0, you can enable iterative index scans, which will automatically scan more of the index until enough results are found (or it reaches hnsw.max_scan_tuples or ivfflat.max_probes).

Iterative scans can use strict or relaxed ordering.

Strict ensures results are in the exact order by distance

Relaxed allows results to be slightly out of order by distance, but provides better recall

With relaxed ordering, you can use a materialized CTE to get strict ordering

Note: + 0 is needed for Postgres 17+

For queries that filter by distance, use a materialized CTE and place the distance filter outside of it for best performance (due to the current behavior of the Postgres executor)

Note: Place any other filters inside the CTE

Since scanning a large portion of an approximate index is expensive, there are options to control when a scan ends.

Specify the max number of tuples to visit (20,000 by default)

Note: This is approximate and does not affect the initial scan

Specify the max amount of memory to use, as a multiple of work_mem (1 by default)

Note: Try increasing this if increasing hnsw.max_scan_tuples does not improve recall

Specify the max number of probes

Note: If this is lower than ivfflat.probes, ivfflat.probes will be used

Use the halfvec type to store half-precision vectors

Index vectors at half precision for smaller indexes

Get the nearest neighbors

Use the bit type to store binary vectors (example)

Get the nearest neighbors by Hamming distance

Also supports Jaccard distance (<%>)

Use expression indexing for binary quantization

Get the nearest neighbors by Hamming distance

Re-rank by the original vectors for better recall

Use the sparsevec type to store sparse vectors

The format is {index1:value1,index2:value2}/dimensions and indices start at 1 like SQL arrays

Get the nearest neighbors by L2 distance

Use together with Postgres full-text search for hybrid search.

You can use Reciprocal Rank Fusion or a cross-encoder to combine results.

Use expression indexing to index subvectors

Get the nearest neighbors by cosine distance

Re-rank by the full vectors for better recall

Use a tool like PgTune to set initial values for Postgres server parameters. For instance, shared_buffers should typically be 25% of the server’s memory. You can find the config file with:

And check individual settings with:

Be sure to restart Postgres for changes to take effect.

Use COPY for bulk loading data (example).

Add any indexes after loading the initial data for best performance.

See index build time for HNSW and IVFFlat.

In production environments, create indexes concurrently to avoid blocking writes.

Use EXPLAIN (ANALYZE, BUFFERS) to debug performance.

To speed up queries without an index, increase max_parallel_workers_per_gather.

If vectors are normalized to length 1 (like OpenAI embeddings), use inner product for best performance.

To speed up queries with an IVFFlat index, increase the number of inverted lists (at the expense of recall).

Vacuuming can take a while for HNSW indexes. Speed it up by reindexing first.

Monitor performance with pg_stat_statements (be sure to add it to shared_preload_libraries).

Get the most time-consuming queries with:

Monitor recall by comparing results from approximate search with exact search.

Scale pgvector the same way you scale Postgres.

Scale vertically by increasing memory, CPU, and storage on a single instance. Use existing tools to tune parameters and monitor performance.

Scale horizontally with replicas, or use Citus or another approach for sharding (example).

Use pgvector from any language with a Postgres client. You can even generate and store vectors in one language and query them in another.

A non-partitioned table has a limit of 32 TB by default in Postgres. A partitioned table can have thousands of partitions of that size.

Yes, pgvector uses the write-ahead log (WAL), which allows for replication and point-in-time recovery.

You can use half-precision vectors or half-precision indexing to index up to 4,000 dimensions or binary quantization to index up to 64,000 dimensions. Other options are indexing subvectors (for models that support it) or dimensionality reduction.

You can use vector as the type (instead of vector(n)).

However, you can only create indexes on rows with the same number of dimensions (using expression and partial indexing):

You can use the double precision[] or numeric[] type to store vectors with more precision.

Optionally, add a check constraint to ensure data can be converted to the vector type and has the expected dimensions.

Use expression indexing to index (at a lower precision):

No, but like other index types, you’ll likely see better performance if they do. You can get the size of an index with:

The query needs to have an ORDER BY and LIMIT, and the ORDER BY must be the result of a distance operator (not an expression) in ascending order.

You can encourage the planner to use an index for a query with:

Also, if the table is small, a table scan may be faster.

The planner doesn’t consider out-of-line storage in cost estimates, which can make a serial scan look cheaper. You can reduce the cost of a parallel scan for a query with:

or choose to store vectors inline:

Results are limited by the size of the dynamic candidate list (hnsw.ef_search), which is 40 by default. There may be even less results due to dead tuples or filtering conditions in the query. Enabling iterative index scans can help address this.

Also, note that NULL vectors are not indexed (as well as zero vectors for cosine distance).

The index was likely created with too little data for the number of lists. Drop the index until the table has more data.

Results can also be limited by the number of probes (ivfflat.probes). Enabling iterative index scans can address this.

Also, note that NULL vectors are not indexed (as well as zero vectors for cosine distance).

Each vector takes 4 * dimensions + 8 bytes of storage. Each element is a single-precision floating-point number (like the real type in Postgres), and all elements must be finite (no NaN, Infinity or -Infinity). Vectors can have up to 16,000 dimensions.

Each half vector takes 2 * dimensions + 8 bytes of storage. Each element is a half-precision floating-point number, and all elements must be finite (no NaN, Infinity or -Infinity). Half vectors can have up to 16,000 dimensions.

Each bit vector takes dimensions / 8 + 8 bytes of storage. See the Postgres docs for more info.

Each sparse vector takes 8 * non-zero elements + 16 bytes of storage. Each element is a single-precision floating-point number, and all elements must be finite (no NaN, Infinity or -Infinity). Sparse vectors can have up to 16,000 non-zero elements.

If your machine has multiple Postgres installations, specify the path to pg_config with:

Then re-run the installation instructions (run make clean before make if needed). If sudo is needed for make install, use:

A few common paths on Mac are:

Note: Replace 18 with your Postgres server version

If compilation fails with fatal error: postgres.h: No such file or directory, make sure Postgres development files are installed on the server.

For Ubuntu and Debian, use:

Note: Replace 18 with your Postgres server version

If compilation fails and the output includes warning: no such sysroot directory on Mac, your Postgres installation points to a path that no longer exists.

Reinstall Postgres to fix this.

By default, pgvector compiles with -march=native on some platforms for best performance. However, this can lead to Illegal instruction errors if trying to run the compiled extension on a different machine.

To compile for portability, use:

If compilation fails with Cannot open include file: 'postgres.h': No such file or directory, make sure PGROOT is correct.

If compilation fails with error C2196: case value '4' already used, make sure you’re using the x64 Native Tools Command Prompt. Then run nmake /F Makefile.win clean and re-run the installation instructions.

If linking fails with unresolved external symbol float_to_shortest_decimal_bufn with Postgres 17.0-17.2, upgrade to Postgres 17.3+.

If installation fails with Access is denied, re-run the installation instructions as an administrator.

Get the Docker image with:

This adds pgvector to the Postgres image (replace 18 with your Postgres server version, and run it the same way).

You can also build the image manually:

If you increase maintenance_work_mem, make sure --shm-size is at least that size to avoid an error with parallel HNSW index builds.

With Homebrew Postgres, you can use:

Note: This only adds it to the postgresql@18 and postgresql@17 formulas

Install from the PostgreSQL Extension Network with:

Debian and Ubuntu packages are available from the PostgreSQL APT Repository. Follow the setup instructions and run:

Note: Replace 18 with your Postgres server version

RPM packages are available from the PostgreSQL Yum Repository. Follow the setup instructions for your distribution and run:

Note: Replace 18 with your Postgres server version

Install the FreeBSD package with:

Install the Alpine package with:

With Conda Postgres, install from conda-forge with:

This method is community-maintained by @mmcauliffe

Download the latest release with Postgres 15+.

pgvector is available on these providers.

Install the latest version (use the same method as the original installation). Then in each database you want to upgrade, run:

You can check the version in the current database with:

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

To enable assertions:

To enable benchmarking:

To show memory usage:

To get k-means metrics:

Resources for contributors

**Examples:**

Example 1 (unknown):
```unknown
NOTICE:  hnsw graph no longer fits into maintenance_work_mem after 100000 tuples
DETAIL:  Building will take significantly more time.
HINT:  Increase maintenance_work_mem to speed up builds.
```

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/commits/master/

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-python/blob/master/examples/hybrid_search/cross_encoder.py

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-gleam

**Contents:**
- pgvector-gleam
- Getting Started
- pog
- Contributing

pgvector examples for Gleam

Follow the instructions for your database library:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/Dockerfile

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/actions

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/forks

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/issues

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-erlang

**Contents:**
- pgvector-erlang
- Getting Started
- epgsql
- Contributing

pgvector examples for Erlang

Follow the instructions for your database library:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-go

**Contents:**
- pgvector-go
- Getting Started
- pgx
- pg
- Bun
- Ent
- GORM
- sqlx
- Reference
  - Vectors

pgvector support for Go

Supports pgx, pg, Bun, Ent, GORM, and sqlx

And follow the instructions for your database library:

Or check out some examples:

Register the types with the connection

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Enable the extension (requires the sql/execquery feature)

Get the nearest neighbors to a vector

Also supports MaxInnerProduct, CosineDistance, L1Distance, HammingDistance, and JaccardDistance

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Get the nearest neighbors to a vector

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a vector from a slice

Create a half vector from a slice

Create a sparse vector from a slice

Or a map of non-zero elements

Note: Indices start at 0

Get the number of dimensions

Get the indices of non-zero elements

Get the values of non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/.editorconfig

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/tags

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-lua

**Contents:**
- pgvector-lua
- Getting Started
- pgmoon
- Reference
  - Half Vectors
  - Sparse Vectors
- History
- Contributing

pgvector support for Lua

And follow the instructions for your database library:

Or check out some examples:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a half vector from a table

Create a sparse vector from a table of non-zero elements

Get the number of dimensions

Get the non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/Makefile

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-fortran

**Contents:**
- pgvector-fortran
- Getting Started
- Libpq-Fortran
- Contributing

pgvector examples for Fortran

Supports Libpq-Fortran

Follow the instructions for your database library:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

Specify the path to libpq if needed:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/tree/master/sql

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-python/blob/master/examples/imagehash/example.py

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/blob/master/.gitignore

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector-nim

**Contents:**
- pgvector-nim
- Getting Started
- db_connector
- Reference
  - Vectors
  - Half Vectors
  - Sparse Vectors
- History
- Contributing

pgvector support for Nim

Supports db_connector

And follow the instructions for your database library:

Or check out an example:

Get the nearest neighbors

Add an approximate index

Use vector_ip_ops for inner product and vector_cosine_ops for cosine distance

Create a vector from a sequence or array

Create a half vector from a sequence or array

Create a sparse vector from a sequence or array

Or a table of non-zero elements

Note: Indices start at 0

Get the number of dimensions

Get the non-zero elements

Everyone is encouraged to help improve this project. Here are a few ways you can help:

To get started with development:

Specify the path to libpq if needed:

Specify the path to libpq if needed:

---

## Search code, repositories, users, issues, pull requests...

**URL:** https://github.com/pgvector/pgvector/branches

---
