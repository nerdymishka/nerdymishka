# NerdyMishka.Security

Many existing .NET libraries that implement things like ChaCha20 or Blake2 do
not implement standard .NET APIs which would enable using those implementations
with streams and other common Scenarios.

Parts of NerdyMishka.Security are implemented to support reading and writing
KeePass files and ship them in re-useable implementations.

Other parts are implemented to support common crypto scenarios for building
applications such as hashing passwords or providing a default encrypt-then-mac
scheme that is versioned so that older implementations can still be read while
enabling the ability to newer versions.

- **NerdyPasswordDeriveBytes** is a back ported version of PasswordDeriveBytes
  from .NET CORE 3.0 which defaults to SHA1 which is deprecated and allows
  substitution of other HMAC hash algorithms such as SHA256. The code from
  Microsoft is licensed under MIT.
- **AesDeriveBytes** borrows from PasswordDeriveBytes and the KeePass AES-KDF to
  create defaults that match that implementation but may be overridden to create
  new variants.
- **ChaCha20** is implemented using the SymmetricAlgorithm base class and
  supports 8, 12, and 20 rounds, 128 and 256 bit keys, allows 8, 12, and 16 byte
  nonces. A 16 byte nonce will use the first 4 bytes to create the value for the
  counter value.

## License

Unless otherwise noted, the code released under the Apache 2.0 license.

Copyright 2016-2020 Nerdy Mishka, Michael Herndon

Licensed under the Apache License, Version 2.0 (the "License"); you may not use
this file except in compliance with the License. You may obtain a copy of the
License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied.

See the License for the specific language governing permissions and limitations
under the License.
