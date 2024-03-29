# Changelog

All notable changes to this project will be documented in this file. See [conventional commits](https://www.conventionalcommits.org/) for commit guidelines.

---
## [1.8.1] - 2024-02-20

### Bug Fixes

- nullability constraint - ([8492607](https://github.com/gerwim/cache/commit/8492607a7b6a34148ec03d041d42439bf2a147e1)) - Gerwim Feiken

### Miscellaneous Chores

- bump version - ([b341f99](https://github.com/gerwim/cache/commit/b341f99fa3507bc8417288eec9777edf4f156798)) - Gerwim Feiken

---
## [1.8.0] - 2024-02-19

### Bug Fixes

-  [**breaking**]value types are now returned as null instead of default if not found - ([7d29faa](https://github.com/gerwim/cache/commit/7d29faa36174e50a1019f0b741c166333ef8909c)) - Gerwim Feiken

### Miscellaneous Chores

- bump StackExchange.Redis - ([81f1ec5](https://github.com/gerwim/cache/commit/81f1ec52828bd40e8d61ac3fb09c9084031ead00)) - Gerwim Feiken
- bump versions - ([60eb27c](https://github.com/gerwim/cache/commit/60eb27cca235044433f311054fe8b78838f4d28b)) - Gerwim Feiken

---
## [1.7.1] - 2024-02-15

### Bug Fixes

- set TypeNameHandling to Objects - ([4023a22](https://github.com/gerwim/cache/commit/4023a22f8b25ede1f21593377446c2041b5ad8c3)) - Gerwim Feiken

### Miscellaneous Chores

- update version to 1.7.1 - ([646c5f2](https://github.com/gerwim/cache/commit/646c5f2f88a33df0852cfe5e3be0772cc4603fb2)) - Gerwim Feiken

---
## [1.7.0] - 2024-02-08

### Features

- allow custom JsonSerializerSettings - ([41d564d](https://github.com/gerwim/cache/commit/41d564db1b58fb7f60936112c22dcafe6d7284b4)) - Gerwim Feiken
- set TypeNameHandling to TypeNameHandling.All - ([3949734](https://github.com/gerwim/cache/commit/394973409eff23d9469839fa0f5bfb02fa55e9e5)) - Gerwim Feiken

### Miscellaneous Chores

- bump version to 1.7.0 - ([b194f86](https://github.com/gerwim/cache/commit/b194f86633f416e4c29927b19315d9c82402edef)) - Gerwim Feiken

---
## [1.6.0] - 2024-02-08

### Bug Fixes

- private setters are now populated when deserializing - ([157908a](https://github.com/gerwim/cache/commit/157908aad6f32be546a53f8dd45c13e426e989f3)) - Gerwim Feiken

### Miscellaneous Chores

- bump versions to 1.6.0 - ([5d6a1ee](https://github.com/gerwim/cache/commit/5d6a1ee8be8cc552a448b2bc7a2e8634d601284d)) - Gerwim Feiken

### Refactoring

- move (de)serialization to BaseCache - ([698c867](https://github.com/gerwim/cache/commit/698c867bf19dc1d8e990082cfb152a88ca19c158)) - Gerwim Feiken

---
## [1.5.0] - 2023-12-22

:warning: key hashing has been removed (see [f7d14aa](https://github.com/gerwim/cache/commit/f7d14aa0a69f6eba461b67fb7f3158213c1c536f)). This means keys were previously bound to a type. E.g. this:

```
cache.Write<string>("key", "value");
```
would not have the key `key` but rather `$"{typeof(T)}-{key}"`. This is **no longer** the case and keys are not unique per type.

Reading a key which has a value of a different type will throw a `InvalidTypeException`.
If you are not sure which type a specific key has, you can use the `Read` method (without specifying it's type) which will return a dynamic.

### Features

- implement ListKeys for Redis - ([3b257f4](https://github.com/gerwim/cache/commit/3b257f48bfcaad1eda6bc2c0e777b759ad74977c)) - Gerwim Feiken
- add generic Read method - ([8862774](https://github.com/gerwim/cache/commit/886277433fa7780b821729692dfb076d2bdb9947)) - Gerwim Feiken
- implement ListKeys for InMemory and Cloudflare - ([41ebdff](https://github.com/gerwim/cache/commit/41ebdff024beeb9fc8599b9b1835526e28a187be)) - Gerwim Feiken
- add deletion of multiple keys - ([8ba33ca](https://github.com/gerwim/cache/commit/8ba33ca0c9681989d7d8320c455becacfe2e5294)) - Gerwim Feiken

### Miscellaneous Chores

- add .editorconfig - ([5c05b5d](https://github.com/gerwim/cache/commit/5c05b5d3858de2df2e7987ceb803225b00510d22)) - Gerwim Feiken
- cleanup - ([2cf2c73](https://github.com/gerwim/cache/commit/2cf2c73729d68e10f0b29572091095e956099d3a)) - Gerwim Feiken
- set lang version to C# 12 - ([674e7e3](https://github.com/gerwim/cache/commit/674e7e3b214ba733720fd2c051ce003adcab78e2)) - Gerwim Feiken
- lock .NET version to 8 - ([06b6f9d](https://github.com/gerwim/cache/commit/06b6f9d277e9b1b30e93bf07d13cda69daac435e)) - Gerwim Feiken
- cleanup - ([54769c7](https://github.com/gerwim/cache/commit/54769c7066d92b5a22c5f22955d45ca76141217b)) - Gerwim Feiken
- fix Sonarcloud scanner - ([b1f3e72](https://github.com/gerwim/cache/commit/b1f3e72fc5038b4756e070c017f7bc6f891648d1)) - Gerwim Feiken
- bump StackExchange.Redis to 2.7.10 - ([e7e3a89](https://github.com/gerwim/cache/commit/e7e3a895b0dd8c715dc670c2dee47028995f546f)) - Gerwim Feiken
- bump all versions to 1.5.0 - ([218be1c](https://github.com/gerwim/cache/commit/218be1caa5ae275ddb4612cb15ec71ac6a613694)) - Gerwim Feiken

### Refactoring

- add ConfigureAwait(false) to async methods - ([8782786](https://github.com/gerwim/cache/commit/878278637f04abc8b54d83771eedce13ae014d10)) - Gerwim Feiken
- revert back to netstandard2.0 - ([48db2f6](https://github.com/gerwim/cache/commit/48db2f6f9ef6d9707fe3626ca0b059a770b8848e)) - Gerwim Feiken
-  [**breaking**]remove key hashing - ([f7d14aa](https://github.com/gerwim/cache/commit/f7d14aa0a69f6eba461b67fb7f3158213c1c536f)) - Gerwim Feiken
- add try/catch for read method in case of mismatching types - ([098dc64](https://github.com/gerwim/cache/commit/098dc646a81b290a7f16be3cbd87342207b0ea97)) - Gerwim Feiken
- add non generic Delete method - ([85f1635](https://github.com/gerwim/cache/commit/85f1635a071cb51ef8452c59f1d1e573d3af3c11)) - Gerwim Feiken
- allow prefix to be null - ([0bbf47e](https://github.com/gerwim/cache/commit/0bbf47e4658f2a0bd92ff6dea26b26bb1b1f293b)) - Gerwim Feiken
- throw exception if retrieving cached value of different type - ([633a558](https://github.com/gerwim/cache/commit/633a558288a7ca96f721b3c095cb604a15bbbe21)) - Gerwim Feiken

### Tests

- update Testcontainers to 3.6.0 - ([a7db0c1](https://github.com/gerwim/cache/commit/a7db0c1c99e651aa103641cbb99bb439952e790a)) - Gerwim Feiken
- update tests - ([a4e35f1](https://github.com/gerwim/cache/commit/a4e35f156dd6d365f1999ae4e50ed6eed99ee2e7)) - Gerwim Feiken

<!-- generated by git-cliff -->
