import React, { useState, useEffect } from 'react';
import styles from '../../styles/SeparataForm.module.scss';
import { useValidateSeparataMutation, useCreateSeparataMutation } from '../../store/separataApi';

export const SeparataForm = () => {
  const PRODUCTS = [
    { id: '3fa85f64-5717-4562-b3fc-2c963f66afa6', name: 'Laptop Pro' },
    { id: '11111111-2222-3333-4444-555555555555', name: 'Mouse Gamer' },
    { id: '99999999-8888-7777-6666-555555555555', name: 'Teclado Mecánico' }
  ];

  // 1. Estado local SÓLO para lo que el usuario digita
  const [formData, setFormData] = useState({ 
    name: '',
    startDate: '',
    endDate: '',
    items: [{ productId: PRODUCTS[0].id, promotionType: 'Percentage', promotionValue: 10 }]
  });
  const [submitStatus, setSubmitStatus] = useState<{ type: 'success' | 'error' | null, message: string }>({ type: null, message: '' });

  const [validate, { data: validationResult, isLoading: isValidating }] = useValidateSeparataMutation();
  const [create, { isLoading: isCreating }] = useCreateSeparataMutation();


  useEffect(() => {
    if (submitStatus.type !== null) setSubmitStatus({ type: null, message: '' });

    const timeoutId = setTimeout(() => {
      if (formData.name && formData.startDate && formData.endDate) {
        // Redux se encarga de la petición HTTP
        validate({
          name: formData.name,
          startDate: new Date(formData.startDate).toISOString(),
          endDate: new Date(formData.endDate).toISOString()
        });
      }
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [formData, validate]);

  const isFormValid = validationResult?.isValid ?? false;
  const serverErrors = validationResult?.errors ?? [];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isFormValid) return;
    setSubmitStatus({ type: null, message: '' });

    try {
      // Usamos la mutación de Redux para crear
      const response = await create({
        name: formData.name,
        startDate: new Date(formData.startDate).toISOString(),
        endDate: new Date(formData.endDate).toISOString()
      }).unwrap(); // unwrap() extrae el payload directo o lanza un error si falla

      setSubmitStatus({ type: 'success', message: "✅ " + response.mensaje });
      setFormData({ name: '', startDate: '', endDate: '', items: [{ productId: PRODUCTS[0].id, promotionType: 'Percentage', promotionValue: 10 }] });

    } catch (error: any) {
      const errorMsg = error?.data?.mensaje || "Error al conectar con el servidor.";
      setSubmitStatus({ type: 'error', message: "❌ " + errorMsg });
    }
  };

  return (
    <div className={styles.formContainer}>
      <h2>Programar Nueva Separata</h2>

      {submitStatus.type === 'success' && <div className={styles.successBanner}>{submitStatus.message}</div>}
      {submitStatus.type === 'error' && <div className={styles.errorBanner}>{submitStatus.message}</div>}

      <form onSubmit={handleSubmit}>
        <div className={styles.formGroup}>
          <label>Nombre de la Campaña</label>
          <input
            type="text"
            placeholder="Ej: Promo Verano 2026"
            value={formData.name}
            onChange={e => setFormData({ ...formData, name: e.target.value })}
          />
        </div>

        <div className={styles.dateRow}>
          <div className={styles.formGroup}>
            <label>Fecha Inicio</label>
            <input
              type="datetime-local"
              value={formData.startDate}
              onChange={e => setFormData({ ...formData, startDate: e.target.value })}
            />
          </div>
          <div className={styles.formGroup}>
            <label>Fecha Fin</label>
            <input
              type="datetime-local"
              value={formData.endDate}
              onChange={e => setFormData({ ...formData, endDate: e.target.value })}
            />
          </div>
        </div>

        {isValidating && <span style={{ color: 'gray', fontSize: '0.8rem' }}>Validando en el servidor...</span>}

        {serverErrors.length > 0 && (
          <div className={styles.errorBanner}>
            <strong>⚠️ Se encontraron problemas:</strong>
            <ul style={{ margin: '5px 0 0 20px', padding: 0 }}>
              {serverErrors.map((error, index) => (
                <li key={index}>{error}</li>
              ))}
            </ul>
          </div>
        )}

        <div className={styles.productsSection}>
          <h3>Productos en Oferta</h3>
          {formData.items.map((item, index) => (
            <div key={index} className={styles.productRow}>
              <select onChange={e => {
                const newItems = [...formData.items];
                newItems[index].productId = e.target.value;
                setFormData({ ...formData, items: newItems });
              }}>
                {PRODUCTS.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
              </select>
              <input
                type="number"
                placeholder="Valor"
                onChange={e => {
                  const newItems = [...formData.items];
                  newItems[index].promotionValue = Number(e.target.value);
                  setFormData({ ...formData, items: newItems });
                }}
              />
            </div>
          ))}
        </div>

        <button
          type="submit"
          className={styles.btnSubmit}
          disabled={!isFormValid || isCreating}
          style={{ opacity: (isFormValid && !isCreating) ? 1 : 0.5, cursor: (isFormValid && !isCreating) ? 'pointer' : 'not-allowed' }}
        >
          {isCreating ? 'Guardando...' : 'Guardar Oferta'}
        </button>
      </form>
    </div>
  );
};