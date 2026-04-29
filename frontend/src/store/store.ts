import { configureStore } from '@reduxjs/toolkit';
import { separataApi } from './separataApi';

export const store = configureStore({
  reducer: {
    // Añadimos el reducer de nuestra API
    [separataApi.reducerPath]: separataApi.reducer,
  },
  // Añadimos el middleware necesario para el caché y las peticiones
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(separataApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;