# CartonCaps

### How will existing users create new referrals using their existing referral code?

API consumers can invoke the endpoint `/api/users/{userId:int}/referral-code` to obtain a referral code for a given user.
The endpoint generates a code if it does not already exist.

If the user exists, the API returns a 200 with the referral code.
If the user does not exists, the API returns a 400 Bad Request.

### How will the app generate referral links for the Share feature?

Consumers may use the `api/users/{userId:int}/referral-tokens [POST]` endpoint to generate tokens
for a given list of users. A token is a GUID, unique for each referral. 

Deep links may embedded this token as a referral token.

If the user Id does not exist, the application returns a 400 Bad Request.

### How will existing users check the status of their referrals?

Applications may use the `api/users/{userId:int}/referral-status [GET]` endpoint to get a list of referrals.

If the user exists, the API returns a 200 OK with list of referrals and their status.
If the user does not exist, the API returns a 400 Bad Request.

### How will the app know where to direct new users after they install the app via a referral?

Applications may use the endpoint `api/referral/{token}/retrieve [GET]` to determine if a token is valid.

If it is valid token, the API returns a 200 with more info on the token, like the referral-code.
If the token is invalid, the API returns a 400 Bad Request.

### Since users may eventually earn rewards for referrals, should we take extra steps to mitigate abuse?

Almost everyone has more than one email address. It is possible for users to refer their alternate email ids and game the system, in order to collect rewards for referrals.
Yes, it is important to take extra steps to mitigate abuse.