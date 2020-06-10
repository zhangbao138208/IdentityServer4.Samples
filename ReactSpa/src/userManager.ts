/* eslint-disable @typescript-eslint/camelcase */
import { createUserManager } from 'redux-oidc';
import { UserManagerSettings } from 'oidc-client';

const userManagerConfig: UserManagerSettings = {
  client_id: 'react-client',
  redirect_uri: 'http://localhost:8084/#/callback',
  response_type: 'token id_token',
  scope:"openid profile api1 email phone",
  authority: 'http://localhost:5001',
  silent_redirect_uri: 'http://localhost:8084/silentRenew.html',
  automaticSilentRenew: true,
  filterProtocolClaims: true,
  loadUserInfo: true,
  monitorSession: true
};

const userManager = createUserManager(userManagerConfig);

export default userManager;